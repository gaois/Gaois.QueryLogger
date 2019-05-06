# Gaois.QueryLogger (ASP.NET Core)

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Gaois.QueryLogger.AspNetCore.svg)](https://www.nuget.org/packages/Gaois.QueryLogger.AspNetCore/)
[![NuGet](https://img.shields.io/nuget/dt/Gaois.QueryLogger.AspNetCore.svg)](https://www.nuget.org/packages/Gaois.QueryLogger.AspNetCore/)

A simple, configurable query logger for ASP.NET Core 2.1+ applications. Find a general introduction to the library [here](https://github.com/gaois/Gaois.QueryLogger).

- [Installation and setup](#installation-and-setup)
  - [Database](#database)
  - [Application](#application)
- [Usage](#usage)
  - [Log a query](#log-a-query)
  - [Associate related queries](#associate-related-queries)
- [Configuration](#configuration)
  - [Globally enable/disable the query logger](#globally-enable-disable-the-query-logger)
  - [Configure application name](#configure-application-name)
  - [Configure IP anonymisation](#configure-ip-anonymisation)
- [Aggregated query logs and log analysis](#aggregated-query-logs-and-log-analysis)

## Installation and setup

### Database

1. Give the application permissions to a database.
2. Run the [SQL script to create the `QueryLogs` table](https://github.com/gaois/Gaois.QueryLogger/tree/master/DBScripts) in the same database.

### Application

Add the NuGet package [Gaois.QueryLogger.AspNetCore](https://www.nuget.org/packages/Gaois.QueryLogger.AspNetCore/) to any ASP.NET Core 2.1+ project.

```cmd
dotnet add package Gaois.QueryLogger.AspNetCore
```

Then, in **Startup.cs**, modify the *ConfigureServices* method by adding a call to `services.AddQueryLogger()`. You will need to add an application name and a connection string for your chosen SQL Server data store also:

```csharp
services.AddQueryLogger(settings =>
{
    settings.ApplicationName = "RecordsApp"
    settings.Store.ConnectionString = configuration.GetConnectionString("query_logger");
});
```
Alternatively, load the configuration from your `appsettings.json` file:

```csharp
services.AddQueryLogger(configuration.GetSection("QueryLogger"));
```

…Or you can mix the two approaches:

```csharp
services.AddQueryLogger(configuration.GetSection("QueryLogger"), settings =>
{
    settings.IsEnabled = !environment.IsDevelopment();
});
```

Now you can add the `using Gaois.QueryLogger` directive to any C# file to access the library's methods and services.

## Usage

Gaois.QueryLogger is implemented in ASP.NET Core using the framework's recommended conventions for [dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection). Pass the query logger dependency to a class constructor via the `IQueryLogger` interface:

```csharp
private readonly IQueryLogger _queryLogger;

public RecordsController(IQueryLogger queryLogger)
{
    _queryLogger = queryLogger;
}

var query = new Query { QueryCategory = "birth_records", QueryTerms = "test", QueryText = Request.Url.Query };
_queryLogger.Log(query);
```

### Log a query

The `Log()` method accepts any number of `Query` objects as parameters.

Example usage:

```csharp
var query = new Query()
{
    QueryCategory = "birth_records",
    QueryTerms = "John Doe Jr.",
    QueryText = Request.Url.Query,
    ResultCount = 27
};

_queryLogger.Log(query);
```

The library automatically obtains the website `Host` and client `IPAddress` properties from the HTTP context. Likewise, if you do not specify a `QueryID` property (in the form of a GUID) one will be created for you. You can, however, overwrite any of these auto-populated values by specifying the relevant property in the `Query` object. See the full list of query data than can be specified [here](https://github.com/gaois/Gaois.QueryLogger/blob/master/LOGDATA.md).

The `Log()` method is ‘fire-and-forget’: queries are added synchronously to a thread-safe log queue which is in turn processed asynchronously by a separate thread in an implementation of the Producer-Consumer pattern. This means that logging adds effectively zero overhead to server response time.

### Associate related queries

If you wish to group related queries together — for example different search queries executed on a single page — assign the same `QueryID` property to each of the associated queries:

```csharp
var queryID = Guid.NewGuid();
var searchText = "John Doe Jr.";

var births = new Query()
{
    QueryID = queryID,
    QueryCategory = "birth_records",
    QueryTerms = searchText
};

var deaths = new Query()
{
    QueryID = queryID,
    QueryCategory = "death_records",
    QueryTerms = searchText
};

_queryLogger.Log(births, deaths);
```

## Configuration

Use the `services.AddQueryLogger()` method in **Startup.cs** to configure the query logger settings.

The rest of this section describes some useful ways you make use of the configuration settings.

### Globally enable/disable the query logger

The query logger is enabled by default. However, there may be occasions or particular environments where, for testing or other purposes, you would prefer to disable the query logger without having to wrap each query command in its own conditional logic. To accomodate this, disable the query logger globally within your application by setting `IsEnabled` to `false`.

```csharp
services.AddQueryLogger(settings =>
{
    settings.IsEnabled = false;
    settings.Store.ConnectionString = Configuration.GetConnectionString("query_logger");
});
```

### Configure application name

Configure your application name globally and avoid having to specify it for each individual `Query` object you create.

```csharp
services.AddQueryLogger(settings =>
{
    settings.ApplicationName = "My Application";
    settings.Store.ConnectionString = Configuration.GetConnectionString("query_logger");
});
```

### Configure IP anonymisation

Use the settings object to configure user IP address anonymisation.

```csharp
services.AddQueryLogger(settings =>
{
    settings.Store.ConnectionString = Configuration.GetConnectionString("query_logger");
    settings.AnonymizeIPAddress = IPAddressAnonymizationLevel.None;
});
```

At present the available anonymisation levels are **None** (no anonymisation is applied) and **Partial** (the last octet of an IPv4 client IP address or the last 80 bits of an IPv6 address are removed).

You can also prevent the logger from collecting IP addresses in the first place by configuring the `StoreClientIPAddress` setting:

```csharp
services.AddQueryLogger(settings =>
{
    settings.Store.ConnectionString = Configuration.GetConnectionString("query_logger");
    settings.StoreClientIPAddress = false;
});
```

When `StoreClientIPAddress` is set to **false** the value **PRIVATE** will be recorded in the `IPAddress` column of your database's query log table. If `StoreClientIPAddress` is set to **true** but a client IP address cannot be obtained from the HTTP context for any reason a value of **UNKNOWN** will be recorded.

## Aggregated query logs and log analysis

In [Fiontar & Scoil na Gaeilge](https://www.gaois.ie), DCU we aggregate summary data from our query log table on monthly basis and store it in a separate database table. We have made the table structure and stored procedures that manage this process available in the [DBScripts](https://github.com/gaois/Gaois.QueryLogger/tree/master/DBScripts) folder in this repository in case they are of use to anyone else. Gaois.QueryLogger also has an `AggregratedQueryLog` entity that corresponds to entries in the aggregated log table. The DBScripts folder also contains some of the more general SQL queries we use to summarise and analyse log data.
