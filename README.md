# Gaois.QueryLogger

A simple, configurable query logger for ASP.NET and ASP.NET Core applications. It is the query logger used internally by [Fiontar & Scoil na Gaeilge](https://www.gaois.ie), Dublin City University, Ireland to log search statistics to SQL Server. Gaois.QueryLogger is a [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) library and it supports applications built with ASP.NET Framework 4.6.1 and ASP.NET Core 2.1 or higher.

**Note:** This is a **prerelease version** for testing purposes. The API surface is now relatively stable and we are focusing on adding additonal features, unit tests and documentation.

- [Package status](#package-status)
- [Features](#features)
- [Documentation](#documentation)
- [Who is using this?](#who-is-using-this)
- [Additional credits](#additional-credits)
- [Roadmap](#roadmap)

## Package status

| Package | NuGet Stable | NuGet Pre-release | Downloads |
| ------- | ------------ | ----------------- | --------- |
| Gaois.QueryLogger | N/A | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Gaois.QueryLogger.svg)](https://www.nuget.org/packages/Gaois.QueryLogger/) | [![NuGet](https://img.shields.io/nuget/dt/Gaois.QueryLogger.svg)](https://www.nuget.org/packages/Gaois.QueryLogger/) |
| Gaois.QueryLogger.AspNetCore | N/A | [![NuGet Pre Release](https://img.shields.io/nuget/vpre/Gaois.QueryLogger.AspNetCore.svg)](https://www.nuget.org/packages/Gaois.QueryLogger.AspNetCore/) | [![NuGet](https://img.shields.io/nuget/dt/Gaois.QueryLogger.AspNetCore.svg)](https://www.nuget.org/packages/Gaois.QueryLogger.AspNetCore/) |

## Features

- Log query terms and associated metadata to SQL Server.
- Metadata include a unique query ID, application name, query category, host server, client IP address, query execution success, query execution duration, result count, and date information.
- Queries can share a GUID, meaning you can group multiple associated queries.
- Add custom metadata to your logs. We use this to store application-specific data in a multi-application log.
- By default, the library partially anonymises user IP addresses by removing the last octet of IPv4 client IP addresses or the last 80 bits of an IPv6 address. This setting can be turned off.
- Performance: logging adds no additional overhead to server response times. Gaois.QueryLogger logs over 100,000 queries per day for us in production with ease.
- Includes its own e-mail notification service that will alert you in the event of any logging errors.

## Documentation

Installation and usage vary slightly according to the target framework. Follow the links below to see the relevant documentation.

- Read the documentation for ASP.NET Framework 4.6.1+ [here](src/Gaois.QueryLogger/).
- Read the documentation for ASP.NET Core 2.1+ [here](src/Gaois.QueryLogger.AspNetCore/).

## Who is using this?

Gaois.QueryLogger is in production use at [www.tearma.ie](https://www.tearma.ie), [www.duchas.ie](https://www.duchas.ie), [www.gaois.ie](https://www.gaois.ie), [www.ainm.ie](https://www.ainm.ie), [www.logainm.ie](https://www.logainm.ie), and [meitheal.logainm.ie](https://meitheal.logainm.ie).

## Roadmap

Planned developments for this library:

- Add unit tests.
- Add sample projects.

## Additional credits

Gaois.QueryLogger makes use of two other third-party open-source libraries:

- [Dapper](https://github.com/StackExchange/Dapper/)
- [AutoMapper](https://automapper.org/)
