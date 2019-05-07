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
	@MonthDiff int = 1
AS
BEGIN
	SET NOCOUNT ON;

	MERGE QueryLogsAggregated AS qla
	USING (
		SELECT ApplicationName, Host, DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - @MonthDiff, 0), COUNT(ID), COUNT(DISTINCT QueryID), COUNT(ExecutedSuccessfully), AVG(CAST(ExecutionTime AS bigint))
		FROM QueryLogs
		WHERE DATEPART(m, LogDate) = DATEPART(m, DATEADD(m, -@MonthDiff, getdate())) AND DATEPART(yyyy, LogDate) = DATEPART(yyyy, DATEADD(m, -@MonthDiff, getdate()))
		GROUP BY ApplicationName, Host
		ORDER BY ApplicationName, Host
	) AS q
		ON LogDate = q.LogDate
	WHEN MATCHED THEN UPDATE
		SET ApplicationName = q.ApplicationName, Host = q.Host, TotalQueries = q.TotalQueries, TotalUniqueQueries = q.TotalUniqueQueries, ExecutedSuccessfully = q.ExecutedSuccessfully, AverageExecutionTime = q.AverageExecutionTime
	WHEN NOT MATCHED THEN
		INSERT (ApplicationName, Host, LogDate, TotalQueries, TotalUniqueQueries, ExecutedSuccessfully, AverageExecutionTime)
		VALUES (q.ApplicationName, q.Host, q.LogDate, q.TotalQueries, q.TotalUniqueQueries, q.ExecutedSuccessfully, q.AverageExecutionTime);
END
GO