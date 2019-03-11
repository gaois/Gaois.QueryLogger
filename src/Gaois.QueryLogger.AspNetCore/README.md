# Gaois.QueryLogger (ASP.NET Core)

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Gaois.QueryLogger.AspNetCore.svg)](https://www.nuget.org/packages/Gaois.QueryLogger.AspNetCore/)
[![NuGet](https://img.shields.io/nuget/dt/Gaois.QueryLogger.AspNetCore.svg)](https://www.nuget.org/packages/Gaois.QueryLogger.AspNetCore/)

A simple, configurable query logger for ASP.NET Core 2.1+ applications. Find a general introduction to the library [here](https://github.com/gaois/Gaois.QueryLogger).

- [Installation and setup](#installation-and-setup)
  - [Database](#database)
  - [Application](#application)
- [Usage](#usage)
  - [Log a query](#log-a-query)
  - [Asynchronous logging](#asynchronous-logging)
  - [Associate related queries](#associate-related-queries)
  - [Configure the query logger settings](#configure-the-query-logger-settings)

## Installation and setup

### Database

1. Give the application permissions to a database.
2. Run the [SQL script to create the `QueryLogs` table](https://github.com/gaois/Gaois.QueryLogger/tree/master/DBScripts) in the same database.

### Application

Add the NuGet package [Gaois.QueryLogger.AspNetCore](https://www.nuget.org/packages/Gaois.QueryLogger.AspNetCore/) to any ASP.NET Core 2.1+ project.

```cmd
dotnet add package Gaois.QueryLogger.AspNetCore
```

Then, in **Startup.cs**, modify the *ConfigureServices* method by adding a call to `services.AddQueryLogger()`. You will need to add a connection string for your chosen SQL Server data store also:

```csharp
services.AddQueryLogger(settings =>
{
    settings.Store.ConnectionString = Configuration.GetConnectionString("query_logger");
});
```

Now you can add the `using Gaois.QueryLogger` directive to any C# file to access the library's methods.

## Usage

Gaois.QueryLogger is implemented in ASP.NET Core using the framework's recommended conventions for [dependency injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection). Pass the query logger dependency to a class constructor via the `IQueryLogger` interface:

```csharp
private readonly IQueryLogger QueryLogger;

public RecordsController(IQueryLogger queryLogger)
{
    QueryLogger = queryLogger;
}

QueryLogger.Log(new Query { ApplicationName = "RecordsApp", QueryCategory = "birth_records", QueryTerms = "test", QueryText = Request.Url.Query });
```

### Log a query

The `Log()` method accepts any number of `Query` objects as parameters.

Example usage:

```csharp
var query = new Query()
{
    ApplicationName = "My Application",
    QueryCategory = "birth_records",
    QueryTerms = "John Doe Jr.",
    QueryText = Request.Url.Query,
    ResultCount = 27
};

QueryLogger.Log(query);
```

The library automatically obtains the website `Host` and client `IPAddress` properties from the HTTP context. Likewise, if you do not specify a `QueryID` property (in the form of a GUID) one will be created for you. You can, however, overwrite any of these automatically-created values by specifying the relevant property in the `Query` object.

### Asynchronous logging

The `LogAsync()` method is provided if you wish to log query data in an asynchronous manner.

### Associate related queries

If you wish to group related queries together — for example different search queries executed on a single page — pass the associated queries the same `QueryID` parameter:

```csharp
var queryID = Guid.NewGuid();
var application = "My Application";
var searchText = "John Doe Jr.";

var births = new Query()
{
    QueryID = queryID,
    ApplicationName = application,
    QueryCategory = "birth_records",
    QueryTerms = searchText
};

var deaths = new Query()
{
    QueryID = queryID,
    ApplicationName = application,
    QueryCategory = "death_records",
    QueryTerms = searchText
};

QueryLogger.Log(births, deaths);
```

### Configure the query logger settings

Use the `services.AddQueryLogger()` method in **Startup.cs** to configure the query logger settings.

#### Globally enable/disable the query logger

The query logger is enabled by default. However, there may be occasions or particular environments where, for testing or other purposes, you would prefer to disable the query logger without having to wrap each query command in its own conditional logic. To accomodate this, disable the query logger globally within your application by setting `IsEnabled` to `false`.

```csharp
services.AddQueryLogger(settings =>
{
    settings.IsEnabled = false;
    settings.Store.ConnectionString = Configuration.GetConnectionString("query_logger");
});
```

#### Configure application name

Configure your application name globally and avoid having to specify it for each individual `Query` object you create.

```csharp
services.AddQueryLogger(settings =>
{
    settings.ApplicationName = "My Application";
    settings.Store.ConnectionString = Configuration.GetConnectionString("query_logger");
});
```

#### Configure IP anonymisation

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
