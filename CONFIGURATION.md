# Gaois.QueryLogger Configuration

The settings below can be configured to suit your particular needs. You can configure the query logger in code (ASP.NET Core only) or in your application configuration file. See the documentation for [ASP.NET Framework](https://github.com/gaois/Gaois.QueryLogger/tree/master/src/Gaois.QueryLogger) and [ASP.NET Core](https://github.com/gaois/Gaois.QueryLogger/tree/master/src/Gaois.QueryLogger.AspNetCore) for implementation details in your target framework.

## Top level

**ApplicationName** (`string`): Specifies a global name for your application that can be used in all queries logged. This means that many apps can all have different application names and share the same SQL database.

**IsEnabled** (`bool`, default: **true**): Specifies whether the application is configured to log queries. Can be useful if you need to temporarily disable logging for any reason or if you wish to enable/disable logging based on environment variables.

**StoreClientIPAddress** (`bool`, default: **true**): Specifies whether the client IP address should be logged. The logged IP address may or may not be subsequently anonymised in part, according to the selected `AnonymizeIPAddress` setting.

**AnonymizeIPAddress** (`IPAddressAnonymizationLevel` enum, default: **IPAddressAnonymizationLevel.Partial**): Sets the level of client IP address anonymization. The possible values are **None** (no anonymization is applied) or **Partial** (removes the last octet of IPv4 addresses or the last 80 bits of an IPv6 address). We came to the conclusion that ‘full’ anonymization — by means of hashing or otherwise — is not something that can be guaranteed and recommend switching the `StoreClientIPAddress` setting to false if you want to ensure that no IP data will be logged.

**AlertInterval** (`integer`, default: **300000**): The interval of time (in milliseconds) the query logger will wait between sending alerts regarding an issue with the logger service. Alerts get sent if the log queue exceeds maximum capacity or if there is an issue writing to the data store — which hopefully will never happen, but we don't want to break your inbox if it does.

## Store

**ConnectionString** (`string`): The connection string for your SQL Server data store.

**ConnectionStringName** (`string`, **ASP.NET Framework only**): The name of the connection string to use from the application’s configuration, e.g. the key of a given entry in the `ConnectionStrings` section of an application's Web.config file.

**MaxQueueRetryTime** (`integer`, default: **300000**): The maximum amount of time (in milliseconds) queries will await enqueuing before being discarded. If you log queue becomes full because of some passing database I/O issue, for example, this reduces the chance that queries will be discarded. However, the higher the retry time the more chance of knock-on performance issues.

**MaxQueueSize** (`integer`, default: **1000**): The maximum possible size of the query log queue before new entries will be blocked. You need to set this with regard to the resources available on your system.

**TableName** (`string`, default: **QueryLogs**): The table name (optionally including schema), e.g. "dbo.QueryLogs" to use when logging queries. Configure this if you want to give your log table a more exotic name than "QueryLogs".

## Email

These settings, if configured, will be used to send you alert e-mails when necessary.

**ToAddress** (`string`): The address to which e-mail messages will be sent (required for alert service).

**FromAddress** (`string`): The address from which e-mail messages will be sent.

**FromDisplayName** (`string`): The display name with which e-mail messages will be sent.

**SMTPHost** (`string`): The SMTP server through which mail will be sent.

**SMTPPort** (`int?`): The port via which mail will be sent (if SMTP server is specified via `SMTPHost` above).

**SMTPUserName** (`string`): The SMTP user name to use, if authenticating.

**SMTPPassword** (`string`): The SMTP password to use, if authenticating.

**SMTPEnableSSL** (`bool`, default: **false**): Whether to use SSL when sending via SMTP.

## ExcludedIPAddresses

Queries associated with the IP addresses in this list will not be logged. Useful if, for instance, you want to prevent queries from an aggressive web crawler or scraper being logged.

**IPAddress** (`string`): The IP address to be excluded (must be a unique value).

**Name** (`string`): The IP address is the entry key, so feel free to set the same name for multiple entries if you want to group multiple IP sources together.
