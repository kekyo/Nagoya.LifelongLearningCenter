/*
 * Nagoya LifelongLearningCenter information fetcher.
 * Copyright (c) 2018 Kouji Matsui, All rights reserved.
 * https://github.com/kekyo/Nagoya.LifelongLearningCenter
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using CenterCLR.Sgml;

namespace Nagoya.LifelongLearningCenter
{
    /// <summary>
    /// Provide information fetcher at Nagoya LifelongLearningCenter.
    /// </summary>
    /// <remarks>This class provides scraping schedules from the Nagoya LifelongLearningCenter seminar rooms.</remarks>
    public static class InformationController
    {
        private static readonly Uri baseUrl =
            new Uri("https://www.suisin.city.nagoya.jp/system/institution/", UriKind.RelativeOrAbsolute);
        private static readonly TimeZoneInfo tzInfo =
            TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

        private static async ValueTask<XDocument> LoadFromUrlAsync(Uri url)
        {
            Console.WriteLine("Downloading: {0}", url);
            using (var httpClient = new HttpClient())
            {
                using (var hs = await httpClient.GetStreamAsync(url))
                {
                    return SgmlReader.Parse(hs);
                }
            }
        }

        private struct Anchor
        {
            public readonly string Value;
            public readonly Uri Url;

            public Anchor(string value, Uri url)
            {
                this.Value = value;
                this.Url = url;
            }
        }

        private static async ValueTask<Anchor[]> LoadAnchorsAsync(Uri url, string xpath)
        {
            var document = await LoadFromUrlAsync(url);
            var aa =
                (from a in document.XPathSelectElements(xpath)
                    where !string.IsNullOrWhiteSpace(a.Value)
                    let href = (string)a.Attribute("href")
                    where !string.IsNullOrWhiteSpace(href) && href.StartsWith("./index.cgi")
                    select new Anchor(a.Value, new Uri(baseUrl, href)))
                .ToArray();
            return aa;
        }

        private static async ValueTask<(XDocument, Anchor[])> LoadMonthAnchorsAsync(Uri url)
        {
            var document = await LoadFromUrlAsync(url);
            var aa =
                (from a in document.XPathSelectElements("//div[@class='datelink']/ul/li/a")
                    where !string.IsNullOrWhiteSpace(a.Value)
                    let href = (string)a.Attribute("href")
                    where !string.IsNullOrWhiteSpace(href) && href.StartsWith("./index.cgi")
                    select new Anchor(a.Value, new Uri(baseUrl, href)))
                .ToArray();
            return (document, aa);
        }

        private struct ScheduleDetail
        {
            public readonly DateTimeOffset Date;
            public readonly TimeSlot TimeSlot;
            public readonly Status Status;

            public ScheduleDetail(DateTimeOffset date, TimeSlot timeSlot, Status status)
            {
                this.Date = date;
                this.TimeSlot = timeSlot;
                this.Status = status;
            }
        }

        private static Status GetStatus(string src)
        {
            if (src.EndsWith("mark01.gif")) return Status.Free;
            if (src.EndsWith("mark02.gif")) return Status.Reserved;
            return Status.Close;
        }

        private static ScheduleDetail[] ExtractSchedules(XDocument document)
        {
            var baseDiv = document.XPathSelectElement("//div[@class='nextArticle']");
            var dateString =
                baseDiv.Element("h3")
                    .Value
                    .Split(new [] { '　' }, StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault();
            var date0 = DateTime.Parse(dateString);
            var dateBase = new DateTimeOffset(date0.Year, date0.Month, 1, 0, 0, 0, tzInfo.BaseUtcOffset);
            var aa =
                (from tableEntry in
                    baseDiv.XPathSelectElements("div[@class='institution02']/table")
                        .Select((table, i) => new { table, half = i })
                    from trEntry in
                    tableEntry.table.XPathSelectElements("tr").Skip(2).Take(3)
                        .Select((tr, i) => new { tr, timeSlot = (TimeSlot)i })
                    from imgEntry in
                    trEntry.tr.XPathSelectElements("td[@class='center']/img")
                        .Select((img, i) => new { img, date = dateBase.AddDays(i + tableEntry.half * 16) })
                    let status = GetStatus((string)imgEntry.img.Attribute("src"))
                    orderby imgEntry.date, trEntry.timeSlot
                    select new ScheduleDetail(imgEntry.date, trEntry.timeSlot, status))
                .ToArray();
            return aa;
        }

        private static async ValueTask<ScheduleDetail[]> LoadSchedulesAsync(Uri url)
        {
            var document = await LoadFromUrlAsync(url);
            return ExtractSchedules(document);
        }

        /// <summary>
        /// Fetch current schedules with reactive.
        /// </summary>
        /// <param name="softDelay">Scraping delays.</param>
        /// <returns>Observable for schedule informations.</returns>
        public static IObservable<Schedule> FetchSchedules(TimeSpan softDelay)
        {
            return
                from centerAnchors in
                    LoadAnchorsAsync(new Uri(baseUrl, "index.cgi"), "//table/tr/td/a")
                    .AsTask()
                    .ToObservable()
                    .Delay(softDelay)
                from centerAnchor in
                    centerAnchors.ToObservable().Delay(softDelay)
                let centerName = centerAnchor.Value
                from roomAnchors in
                    LoadAnchorsAsync(centerAnchor.Url, "//th[@class='roomth']/a")
                    .AsTask()
                    .ToObservable()
                    .Delay(softDelay)
                from roomAnchor in
                    roomAnchors
                    .ToObservable()
                    .Delay(softDelay)
                let roomName = roomAnchor.Value
                from roomByMonthAnchorsEntry in
                    LoadMonthAnchorsAsync(roomAnchor.Url)
                    .AsTask()
                    .ToObservable()
                    .Delay(softDelay)
                let roomByMonthAnchor0 = roomByMonthAnchorsEntry.Item1
                let roomByMonthAnchors = roomByMonthAnchorsEntry.Item2
                from scheduleDetails in
                    new[] { ExtractSchedules(roomByMonthAnchor0) }
                    .ToObservable()
                    .Concat(
                        roomByMonthAnchors
                        .ToObservable()
                        .Delay(softDelay)
                        .SelectMany(roomByMonthAnchor =>
                            LoadSchedulesAsync(roomByMonthAnchor.Url)
                            .AsTask()
                            .ToObservable()
                            .Delay(softDelay)))
                from scheduleDetail in scheduleDetails
                select new Schedule(centerName, roomName, scheduleDetail.Date, scheduleDetail.TimeSlot, scheduleDetail.Status);
        }

        /// <summary>
        /// Fetch current schedules with reactive.
        /// </summary>
        /// <returns>Observable for schedule informations.</returns>
        /// <remarks>Soft delay defined for 3 seconds every fetch executions.</remarks>
        public static IObservable<Schedule> FetchSchedules()
        {
            return FetchSchedules(TimeSpan.FromSeconds(3));
        }

        /// <summary>
        /// Fetch current schedules with asynchronous.
        /// </summary>
        /// <param name="softDelay">Scraping delays.</param>
        /// <returns>Schedule informations.</returns>
        public static Task<Schedule[]> FetchSchedulesAsync(TimeSpan softDelay)
        {
            return FetchSchedules(softDelay).ToArray().ToTask();
        }

        /// <summary>
        /// Fetch current schedules with asynchronous.
        /// </summary>
        /// <returns>Schedule informations.</returns>
        /// <remarks>Soft delay defined for 3 seconds every fetch executions.</remarks>
        public static Task<Schedule[]> FetchSchedulesAsync()
        {
            return FetchSchedules().ToArray().ToTask();
        }
    }
}
