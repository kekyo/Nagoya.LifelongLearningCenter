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
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Nagoya.LifelongLearningCenter
{
    class Program
    {
        private static async Task MainAsync(string[] args)
        {
            // For debugging purpose
            //InformationController.Fetching += (_, e) => Console.WriteLine($"Fetching: {e.Url}");
            //InformationController.Fetched += (_, e) => Console.WriteLine($"Fetched: {e.Url}");

            // Filter by free room at afternoon or evening in Saturday, 2018. And groups by date and time slot.
            await InformationController.FetchSchedulesOnConcurrent()
                .Where(schedule =>
                     schedule.Date.Year == 2018 &&
                     schedule.Date.DayOfWeek == DayOfWeek.Saturday &&
                     (schedule.TimeSlot == TimeSlot.Afternoon || schedule.TimeSlot == TimeSlot.Evening) &&
                     schedule.Status == Status.Free)
                .ForEachAsync(schedule => Console.WriteLine(schedule.ToString()));
        }

        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }
    }
}
