# Gaois.QueryLogger

[![NuGet](https://img.shields.io/badge/nuget-0.5.3--alpha-blue.svg)](https://www.nuget.org/packages/Gaois.QueryLogger/)

A simple, configurable query logger for ASP.NET and ASP.NET Core applications. It is the query logger used internally by [Fiontar & Scoil na Gaeilge](https://www.gaois.ie), Dublin City University, Ireland to log search statistics to SQL Server. In the future, the library will support additional backend data stores.

Gaois.QueryLogger is a [.NET Standard 1.4](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) library and it supports applications built with ASP.NET Framework 4.6.1 or ASP.NET Core 2.1 or higher.  SQL commands executed within the query logger are handled by the [Dapper](https://github.com/StackExchange/Dapper/) micro-ORM for consistency with our other libraries.

**Note:** This is a **prerelease version** for testing purposes. Expect some breaking changes and renamed API methods before we reach a 1.0 release.

- [Features](#features)
- [Documentation](#documentation)
- [Who is using this?](#who-is-using-this)
- [Roadmap](#roadmap)

## Features

- Log query terms and associated metadata to SQL Server.
- Metadata include a unique query ID, application name, query category, host server, client IP address, query execution success, query execution duration, result count, and date information.
- Queries can share a GUID, meaning you can group multiple associated queries.
- By default, the library partially anonymises user IP addresses by removing the last octet of IPv4 client IP addresses. This setting can be turned off.


## Documentation

Installation and usage vary slightly according to the target framework. Follow the links below to see the relevant documentation.

- Read the documentation for .NET Framework 4.6.1+ [here](src/Gaois.QueryLogger/).
- Read the documentation for ASP.NET Core 2.1+ [here](src/Gaois.QueryLogger.AspNetCore/).

## Who is using this?

Gaois.QueryLogger has been implemented on several of Fiontar & Scoil na Gaeilge's project websites, including [www.dúchas.ie](https://www.duchas.ie/), [www.gaois.ie](https://www.gaois.ie), [www.logainm.ie](https://www.logainm.ie), and [meitheal.logainm.ie](https://meitheal.logainm.ie).

## Roadmap

Planned developments for this library:

- Add unit tests.
- Add sample project.
- Add options to access the data store settings from a configuration file.
- Provide Sqlite implementation.
- Provide additional IP anonymisation options (e.g. Hashed).