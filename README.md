# Nagoya LifelongLearningCenter information fetcher.

## Status

| Title | Status |
|:----|:----|
| NuGet | [![NuGet](https://img.shields.io/nuget/v/Nagoya.LifelongLearningCenter.svg?style=flat)](https://www.nuget.org/packages/Nagoya.LifelongLearningCenter) |

## What is this?
* Nagoya LifelongLearningCenter information fetcher.
  * [The learning center is in Nagoya, Japan.](https://www.suisin.city.nagoya.jp/system/institution/index.cgi)
* Totally perfect concurrent fetching process using reactive extension.

```csharp
// For debugging purpose
InformationController.Fetching += (_, e) => Console.WriteLine($"Fetching: {e.Url}");
InformationController.Fetched += (_, e) => Console.WriteLine($"Fetched: {e.Url}");

await InformationController.FetchSchedulesOnConcurrent()
    .ForEachAsync(schedule => Console.WriteLine(schedule.ToString()));
```

* The `Schedule` type contains these items:
  * The center name.
  * The room name in the center.
  * This schedule date (contains only date, no time details).
  * This schedule time slot.
  * This schedule status.

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

## TODO
 Improvements more easier/effective interfaces.

## License
* Copyright (c) 2018 Kouji Matsui (@kozy_kekyo)
* Under Apache v2 http://www.apache.org/licenses/LICENSE-2.0

## History
* 0.7.2:
  * Add xml comment files.
* 0.7.1:
  * First release
  * Supported .NET Standard 2.0, .NET Core 2.0 and .NET Framework 4.6.
