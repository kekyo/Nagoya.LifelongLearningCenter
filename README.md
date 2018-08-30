# Nagoya LifelongLearningCenter information fetcher.

（名古屋市生涯学習センター情報取得ライブラリ for .NET）

## Status

| Title | Status |
|:----|:----|
| NuGet | [![NuGet](https://img.shields.io/nuget/v/Nagoya.LifelongLearningCenter.svg?style=flat)](https://www.nuget.org/packages/Nagoya.LifelongLearningCenter) |

## What is this?
* Nagoya LifelongLearningCenter information fetcher.
  * [The learning center is in Nagoya, Japan.](https://www.suisin.city.nagoya.jp/system/institution/index.cgi)
  * [名古屋市生涯学習センター](https://www.suisin.city.nagoya.jp/system/institution/index.cgi)の空室状況を簡単に取得することが出来る、.NETのライブラリです。
* Totally perfect concurrent fetching process using reactive extension.
  * 完全コンカレント動作に対応しています。インターフェイスはReactive extension、又は部分的にTaskによる非同期取得を選択できます。

Example below （サンプルコード）:

```csharp
async Task FetchInformationsAsync()
{
  // For debugging purpose （動作をデバッグするのに、これらのイベントを使えます。必要なければ不要）
  InformationController.Fetching += (_, e) => Console.WriteLine($"Fetching: {e.Url}");
  InformationController.Fetched += (_, e) => Console.WriteLine($"Fetched: {e.Url}");

  // Fetch informations by reactive extension（Reactive extensionを使って、完全コンカレントに情報を取得・表示します）
  await InformationController.FetchSchedulesOnConcurrent()
      .ForEachAsync(schedule => Console.WriteLine(schedule.ToString()));
}
```

* The `Schedule` type contains these items （Schedule型には以下の情報を含みます）:
  * The center name. （センター名）
  * The room name in the center. （部屋名）
  * This schedule date (contains only date, no time details). （このスケジュールの日付・時間はダミー）
  * This schedule time slot. （このスケジュールの時間帯）
  * This schedule status. （このスケジュールのステータス）

次のような定義です:

```csharp
public struct Schedule
{
    public readonly string CenterName;
    public readonly string RoomName;
    public readonly DateTimeOffset Date;
    public readonly TimeSlot TimeSlot;
    public readonly Status Status;
}
```

* We can use filtering and/or complex calculation using LINQ/Rx.
  * LINQやRxを使って、好きなように絞り込んでください。例えば:

```csharp
// Filter by free room at afternoon or evening in Saturday, 2018. And groups by date and time slot.
// （2018年で、土曜日の午後と夜間の空き部屋に絞り込む）
await InformationController.FetchSchedulesOnConcurrent()
    .Where(schedule =>
         schedule.Date.Year == 2018 &&
         schedule.Date.DayOfWeek == DayOfWeek.Saturday &&
         (schedule.TimeSlot == TimeSlot.Afternoon || schedule.TimeSlot == TimeSlot.Evening) &&
         schedule.Status == Status.Free)
    .ForEachAsync(schedule => Console.WriteLine(schedule.ToString()));
```

## TODO
 Improvements more easier/effective interfaces.

## License
* Copyright (c) 2018 Kouji Matsui (@kozy_kekyo)
* Under Apache v2 http://www.apache.org/licenses/LICENSE-2.0

## History
* 0.7.3:
  * Set user agent to IE 10.0.
* 0.7.2:
  * Add xml comment files.
* 0.7.1:
  * First release
  * Supported .NET Standard 2.0, .NET Core 2.0 and .NET Framework 4.6.
