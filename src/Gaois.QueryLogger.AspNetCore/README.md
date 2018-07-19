# Gaois.QueryLogger (ASP.NET Core)

A simple, configurable query logger for ASP.NET Core 2.0+ applications. Find a general introduction to the library here.

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

Add the NuGet package [Gaois.QueryLogger.AspNetCore](https://www.nuget.org/packages/Gaois.QueryLogger.AspNetCore/) to any ASP.NET Core 2.0+ project.

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

QueryLogger.Log(new Query { ApplicationName = "RecordsApp", QueryCategory = "birth_records", QueryText = "test" });
```

### Log a query

The `Log()` method accepts any number of `Query` objects as parameters.

Example usage:

```csharp
var query = new Query()
{
    ApplicationName = "My Application",
    QueryCategory = "birth_records",
    QueryText = "John Doe Jr.",
    ResultCount = 27
};

QueryLogger.Log(query);
```

The library automatically obtains the website `Host` and client `IPAddress` properties from the HTTP context. Likewise, if you do not specify a `QueryID` property (in the form of a GUID) one will be created for you. You can, however, overwrite any of these automatically-created values by specifying the relevant property in the `Query` object.

### Asynchronous logging

The `LogAsync()` method is provided if you wish to log query data in an asynchronous manner. `LogAsync()` returns the number of queries successfully logged as an integer. It can be used the same as the synchronous `Log()` method in all other respects.

### Associate related queries

If you wish to group related queries together — for example different search queries executed on a single page — pass the associated queries the same `QueryID` parameter:

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

QueryLogger.Log(births, deaths);
```

### Configure the query logger settings

Use the `services.AddQueryLogger()` method in **Startup.cs** to configure the query logger settings.

#### Configure IP anonymisation

The `Log()` method has an overload that accepts a `QueryLoggerSettings` object. Use the settings object to configure user IP address anonymisation.

```csharp
services.AddQueryLogger(settings =>
{
    settings.Store.ConnectionString = Configuration.GetConnectionString("query_logger");
    settings.AnonymizeIPAddress = IPAddressAnonymizationLevel.None;
});
```

At present the available anonymisation levels are **None** (no anonymisation is applied) and **Partial** (the last octet of an IPv4 client IP address is removed).

You can also prevent the logger from collecting IP addresses in the first place by configuring the `StoreClientIPAddress` setting:

```csharp
services.AddQueryLogger(settings =>
{
    settings.Store.ConnectionString = Configuration.GetConnectionString("query_logger");
    settings.StoreClientIPAddress = false;
});
```