# Gaois.QueryLogger (ASP.NET Framework)

[![NuGet Pre Release](https://img.shields.io/nuget/vpre/Gaois.QueryLogger.svg)](https://www.nuget.org/packages/Gaois.QueryLogger/)
[![NuGet](https://img.shields.io/nuget/dt/Gaois.QueryLogger.svg)](https://www.nuget.org/packages/Gaois.QueryLogger/)

A simple, configurable query logger for ASP.NET Framework 4.6.1+ applications. Find a general introduction to the library [here](../../).

- [Installation and setup](#installation-and-setup)
  - [Database](#database)
  - [Application](#application)
- [Usage](#usage)
  - [Log a query](#log-a-query)
  - [Asynchronous logging](#asynchronous-logging)
  - [Associate related queries](#associate-related-queries)
  - [Configure IP anonymisation](#configure-ip-anonymisation)

## Installation and setup

### Database

1. Give the application permissions to a database.
2. Run the [SQL script to create the `QueryLogs` table](https://github.com/dcufsg/Gaois.QueryLogger/tree/master/DBScripts) in the same database.

### Application

Add the NuGet package [Gaois.QueryLogger](https://www.nuget.org/packages/Gaois.QueryLogger/) to any ASP.NET Framework 4.6.1+ project.

```cmd
Install-Package Gaois.QueryLogger
```

Add the `using Gaois.QueryLogger` directive to any C# file to access the library's static methods.

## Usage

### Log a query

The `QueryLogger.Log()` method accepts a SQL Server database connection string and any number of `Query` objects as parameters.

Example usage:

```csharp
var query = new Query()
{
    ApplicationName = "My Application",
    QueryCategory = "land_records",
    QueryText = this.search,
    ResultCount = this.records.Count
};

QueryLogger.Log(Config.ConnectionString, query);
```

The library automatically obtains the website `Host` and client `IPAddress` properties from the HTTP context. Likewise, if you do not specify a `QueryID` (in the form of a GUID) in the `Query` object one will be created for you. You can, however, overwrite any of these automatically-created values by specifying the relevant property in the `Query` object.

### Asynchronous logging

The `LogAsync()` method is provided if you wish to log query data in an asynchronous manner. `LogAsync()` returns the number of queries successfully logged as an integer. It can be used the same as the synchronous `Log()` method in all other respects.


### Associate related queries

If you wish to group related queries together � for example different search queries executed on a single page � pass the associated queries the same `QueryID` parameter:

```csharp
Guid queryID = Guid.NewGuid();
string application = "My Application";
string searchText = "John Doe Jr.";

var births = new Query()
{
    QueryID = queryID,
    ApplicationName = application,
    QueryCategory = "birth_records",
    QueryText = searchText
};

var deaths = new Query()
{
    QueryID = queryID,
    ApplicationName = application,
    QueryCategory = "death_records",
    QueryText = searchText
};

QueryLogger.Log(connectionString, births, deaths);
```

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
    QueryText = "John Doe Jr."
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