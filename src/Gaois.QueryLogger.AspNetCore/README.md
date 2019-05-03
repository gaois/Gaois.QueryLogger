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

The library automatically obtains the website `Host` and client `IPAddress` properties from the HTTP context. Likewise, if you do not specify a `QueryID` property (in the form of a GUID) one will be created for you. You can, however, overwrite any of these auto-populated values by specifying the relevant property in the `Query` object.

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

### Configure the query logger settings

See below for a list of available settings and their descriptions.

#### Top level

**ApplicationName** (`string`): Specifies a global name for your application that can be used in all queries logged. This means that many apps can all have different application names and share the same SQL database.

**IsEnabled** (`bool`, default: **true**): Specifies whether the application is configured to log queries. Can be useful if you need to temporarily disable logging for any reason or if you wish to enable/disable logging based on environment variables.

**StoreClientIPAddress** (`bool`, default: **true**): Specifies whether the client IP address should be logged. The logged IP address may or may not be subsequently anonymised in part, according to the selected `AnonymizeIPAddress` setting.

**AnonymizeIPAddress** (`IPAddressAnonymizationLevel` enum, default: **IPAddressAnonymizationLevel.Partial**): Sets the level of client IP address anonymization. The possible values are **None** (no anonymization is applied) or **Partial** (removes the last octet of IPv4 addresses or the last 80 bits of an IPv6 address). We came to the conclusion that ‘full’ anonymization — by means of hashing or otherwise — is not something that can be guaranteed and recommend switching the `StoreClientIPAddress` setting to false if you want to ensure that no IP data will be logged.

**AlertInterval** (`integer`, default: **300000**): The interval of time (in milliseconds) the query logger will wait between sending alerts regarding an issue with the logger service. Alerts get sent if the log queue exceeds maximum capacity or if there is an issue writing to the data store — which hopefully will never happen, but we don't want to break your inbox if it does.

#### Store

**ConnectionString** (`string`): The connection string for your SQL Server data store.

**MaxQueueRetryTime** (`integer`, default: **300000**): The maximum amount of time (in milliseconds) queries will await enqueuing before being discarded. If you log queue becomes full because of some passing database I/O issue, for example, this reduces the chance that queries will be discarded. However, the higher the retry time the more chance of knock-on performance issues.

**MaxQueueSize** (`integer`, default: **1000**): The maximum possible size of the query log queue before new entries will be blocked. You need to set this with regard to the resources available on your system.

**TableName** (`string`, default: **QueryLogs**): The table name (optionally including schema), e.g. "dbo.QueryLogs" to use when logging queries. Configure this if you want to give your log table a more exotic name than "QueryLogs".

#### Email

These settings, if configured, will be used to send you alert e-mails when necessary.

**ToAddress** (`string`): The address to which e-mail messages will be sent (required for alert service).

**FromAddress** (`string`): The address from which e-mail messages will be sent.

**FromDisplayName** (`string`): The display name with which e-mail messages will be sent.

**SMTPHost** (`string`): The SMTP server through which mail will be sent.

**SMTPPort** (`int?`): The port via which mail will be sent (if SMTP server is specified via `SMTPHost` above).

**SMTPUserName** (`string`): The SMTP user name to use, if authenticating.

**SMTPPassword** (`string`): The SMTP password to use, if authenticating.

**SMTPEnableSSL** (`bool`, default: **false**): Whether to use SSL when sending via SMTP.

#### ExcludedIPAddresses

Queries associated with the IP addresses in this list will not be logged. Useful if, for instance, you want to prevent queries from an aggressive web crawler or scraper being logged.

**IPAddress** (`string`): The IP address to be excluded (must be a unique value).

**Name** (`string`): The IP address is the entry key, so feel free to set the same name for multiple entries if you want to group multiple IP sources together.

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
