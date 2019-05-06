# Adding query data

## Data

**QueryID** (`Guid`): A unique ID that represents a specific query or group of queries. A query ID will be created automatically if not specified in code.

**ApplicationName** (`string`): The name of the application that executes the query. The application name will be retrieved from your configuration settings if not specified in code.

**QueryCategory** (`string`): Relates the query to a category specified by the application. Useful if, for example, you have multiple search types within one application (optional).

**QueryTerms** (`string`): The query term(s) or text content of the query (optional).

**QueryText** (`string`): The raw query text, such as a query string or POST-ed form field (optional).

**Host** (`string`): The application host domain, e.g. www.example.com. Will be automatically obtained from the current HTTP context if not otherwise specified.

**IPAddress** (`string`): The client IP address. Will be automatically obtained from the current HTTP context if not otherwise specified.

**ExecutedSuccessfully** (`bool`, default: **true**): Records whether the query executed successfully.

**ExecutionTime** (`int?`): The query execution time in milliseconds (optional).

**ResultCount** (`int?`): The count of results returned by the query (optional).

**LogDate** (`DateTime`): The date and time of query logging. A value will be automatically generated for this property at time of logging if not otherwise specified.

**JsonData** (`string`): Record additional data in JSON format. Useful for storing application- or query-specific data in a multi-application environment, for example.

## Methods

**`ToJson()`** (`string`): Returns a JSON representation of a `Query` object.

**`FromJson(string json)`** (`string`): Deserializes the provided JSON into a `Query` object.
