USE [YOUR_DATABASE_NAME]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Ronan Doherty
-- Create date: 2019-05-04
-- Description:	Aggregates and stores query log data for the previous month per application/host 
-- =============================================
CREATE PROCEDURE AggregateQueryLogsMonthly
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO QueryLogsAggregated (ApplicationName, Host, LogDate, TotalQueries, TotalUniqueQueries, ExecutedSuccessfully, AverageExecutionTime)
		SELECT ApplicationName, Host, DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0), COUNT(ID), COUNT(DISTINCT QueryID), COUNT(ExecutedSuccessfully), AVG(CAST(ExecutionTime AS bigint))
		FROM QueryLogs
		WHERE DATEPART(m, LogDate) = DATEPART(m, DATEADD(m, -1, getdate())) AND DATEPART(yyyy, LogDate) = DATEPART(yyyy, DATEADD(m, -1, getdate()))
		GROUP BY ApplicationName, Host
		ORDER BY ApplicationName, Host;
END
GO