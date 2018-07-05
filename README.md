# Gaois.QueryLogger

[![NuGet](https://img.shields.io/badge/nuget-0.5.0--alpha-blue.svg)](https://www.nuget.org/packages/Gaois.QueryLogger/)

A simple, configurable query logger for ASP.NET and ASP.NET Core applications. It is the query logger used internally by [Fiontar & Scoil na Gaeilge](https://www.gaois.ie), Dublin City University, Ireland to log search statistics to SQL Server. In the future, the library will support additional backend data stores.

Gaois.QueryLogger is a [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) library and it supports applications built with ASP.NET Framework 4.6.1 or ASP.NET Core 2.0 or higher.  SQL commands executed within the query logger are handled by the [Dapper](https://github.com/StackExchange/Dapper/) micro-ORM for consistency with our other libraries.

**Note:** This is a **prerelease version** for testing purposes. Expect some breaking changes and renamed API methods before we reach a 1.0 release.

- [Features](#features)
- [Install and setup](#install-and-setup)
- [Usage](#usage)
- [Who is using this?](#who-is-using-this)
- [Roadmap](#roadmap)

## Features

- Log query terms and associated metadata to SQL Server.
- Metadata include a unique query ID, a query category, the host server, user IP address, and date information.
- Queries can share a GUID, meaning you can group multiple associated queries.
- By default, the library partially anonymises user IP addresses by removing the last octet of IPv4 user IP addresses. This setting can be turned off.

## Install and setup

### Database

1. Give the application permissions to a database.
2. Run the SQL script to create the `QueryLogs` table in the same database.

### Application

Add the NuGet package [Gaois.QueryLogger](https://www.nuget.org/packages/Fiontar.Localization/) to any ASP.NET Framework 4.6.1+ or ASP.NET Core 2.0+ project.

```cmd
dotnet add package Gaois.QueryLogger
```

Add `using Gaois.QueryLogger;` at the top of any C# file to access the library's static methods.

## Usage

### Log a query

The `QueryLogger.Log()` method accepts a query object and a SQL Server database connection string:

```csharp
var queryData = new Query()
{
    QueryID = Guid.NewGuid(),
    ApplicationName = "My Application",
    QueryCategory = "people_search",
    QueryText = "John Doe Jr.",
    Host = HttpContext.Request.Host.ToString();,
    IPAddress = HttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()
};

QueryLogger.Log(queryData, connectionString);
```

### Associate related queries

If you wish to associate

```csharp
var queryData = new Query()
{
    QueryID = Guid.NewGuid(),
    ApplicationName = "My Application",
    QueryCategory = "people_search",
    QueryText = "John Doe Jr.",
    Host = HttpContext.Request.Host.ToString();,
    IPAddress = HttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()
};

QueryLogger.Log(queryData, connectionString);
```

### Configure IP anonymisation

The `QueryLogger.Log()` method has an overload that accepts a `QueryLoggerSettings` object. Use the settings object to configure user IP address anonymisation.

```csharp

var settings = new QueryLoggerSettings()
{
    AnonymizeIPAddress = IPAddressAnonymizationLevel.None
};

var queryData = new Query()
{
    QueryID = Guid.NewGuid(),
    ApplicationName = "My Application",
    QueryCategory = "people_search",
    QueryText = "John Doe Jr.",
    Host = HttpContext.Request.Host.ToString();,
    IPAddress = HttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()
};

QueryLogger.Log(queryData, settings, connectionString);
```

At present the available anonymisation levels are

## Who is using this?

Gaois.QueryLogger has been implemented on several of Fiontar & Scoil na Gaeilge's project websites, including [www.dúchas.ie](https://www.duchas.ie/), [www.gaois.ie](https://www.gaois.ie), [www.logainm.ie](https://www.logainm.ie), and [meitheal.logainm.ie](https://meitheal.logainm.ie).

## Roadmap

Planned developments for this library:

- Provide async methods.
- Add options to access the data store settings from a configuration file, so you won't have to pass a connection string to the `Log()` method.
- Provide Sqlite implementation.
- Provide additional IP anonymisation options (e.g. Hashed).
- Consider separate libraries that target .NET Core and .NET Framework as this would reduce the number of parameters, such as `Host` and `IPAddress`, that need to be passed to the `Log()` method via the `Query` object. This is due to different implementations of `HttpContext` in the two frameworks.