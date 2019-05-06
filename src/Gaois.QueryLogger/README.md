# Gaois.QueryLogger (ASP.NET Framework)

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Gaois.QueryLogger.svg)](https://www.nuget.org/packages/Gaois.QueryLogger/)
[![NuGet](https://img.shields.io/nuget/dt/Gaois.QueryLogger.svg)](https://www.nuget.org/packages/Gaois.QueryLogger/)

A simple, configurable query logger for ASP.NET Framework 4.6.1+ applications. Find a general introduction to the library [here](https://github.com/gaois/Gaois.QueryLogger).

- [Installation and setup](#installation-and-setup)
  - [Database](#database)
  - [Application](#application)
- [Usage](#usage)
  - [Log a query](#log-a-query)
  - [Associate related queries](#associate-related-queries)
- [Configuration](#configuration)
  - [Globally enable/disable the query logger](#globally-enabledisable-the-query-logger)
  - [Configure application name](#configure-application-name)
  - [Configure IP anonymisation](#configure-ip-anonymisation)
- [Aggregated query logs and log analysis](#aggregated-query-logs-and-log-analysis)

## Installation and setup

### Database

1. Give the application permissions to a database.
2. Run the [SQL script to create the `QueryLogs` table](https://github.com/gaois/Gaois.QueryLogger/tree/master/DBScripts) in the same database.

### Application

Add the NuGet package [Gaois.QueryLogger](https://www.nuget.org/packages/Gaois.QueryLogger/) to any ASP.NET Framework 4.6.1+ project.

```cmd
Install-Package Gaois.QueryLogger
```

Then, configure the query logger in your **Web.config** file. You will need to add an application name and a connection string for your chosen SQL Server data store:

```xml
<QueryLogger applicationName="RecordsApp" isEnabled="true">
  <Store connectionString="Server=localhost;Database=recordsappdb;Trusted_Connection=True;" />
  <Email toAddress="me@test.ie" />
</QueryLogger>
```

Now you can add the `using Gaois.QueryLogger` directive to any C# file to access the library's methods and services.

## Usage

### Log a query

The `QueryLogger.Log()` method accepts any number of `Query` objects as parameters.

Example usage:

```csharp
var query = new Query()
{
    QueryCategory = "birth_records",
    QueryTerms = "John Doe Jr.",
    QueryText = Request.Url.Query,
    ResultCount = 27
};

QueryLogger.Log(query);
```

The library automatically obtains the website `Host` and client `IPAddress` properties from the HTTP context. Likewise, if you do not specify a `QueryID` (in the form of a GUID) in the `Query` object one will be created for you. You can, however, overwrite any of these automatically-created values by specifying the relevant property in the `Query` object. See the full list of query data than can be specified [here](https://github.com/gaois/Gaois.QueryLogger/blob/master/LOGDATA.md).

The `Log()` method is ‘fire-and-forget’: queries are added synchronously to a thread-safe log queue which is in turn processed asynchronously by a separate thread in an implementation of the Producer-Consumer pattern. This means that logging adds effectively zero overhead to server response time.

### Associate related queries

If you wish to group related queries together — for example different search queries executed on a single page — pass the associated queries the same `QueryID` parameter:

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

QueryLogger.Log(births, deaths);
```

## Configuration

### Configure IP anonymisation

The `Log()` method has an overload that accepts a `QueryLoggerSettings` object. Use the settings object to configure user IP address anonymisation.

```csharp
var settings = new QueryLoggerSettings()
{
    AnonymizeIPAddress = IPAddressAnonymizationLevel.None
};

var queryData = new Query()
{
    QueryID = Guid.NewGuid(),
    ApplicationName = "My Application",
    QueryCategory = "birth_records",
    QueryTerms = "John Doe Jr.",
    QueryText = Request.Url.Query
};

QueryLogger.Log(connectionString, settings, queryData);
```

At present the available anonymisation levels are **None** (no anonymisation is applied) and **Partial** (the last octet of an IPv4 client IP address or the last 80 bits of an IPv6 address are removed).

You can also prevent the logger from collecting IP addresses in the first place by configuring the `StoreClientIPAddress` setting:

```csharp
var settings = new QueryLoggerSettings()
{
    StoreClientIPAddress = false
};
```
