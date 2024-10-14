DROP PROCEDURE IF EXISTS dbo.DropAllTables
GO
CREATE PROCEDURE dbo.DropAllTables 
AS
BEGIN
    BEGIN TRY
	  DROP TABLE dbo.CheckInHistory;
	  DROP TABLE dbo.CheckIn;
      DROP TABLE dbo.ActiveSubscriptions;
	  DROP TABLE dbo.OfferedSubscriptions;
	  DROP TABLE dbo.Subscribers;
    END TRY
    BEGIN CATCH
        SELECT
            ERROR_NUMBER() AS ErrorNumber, 
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END;
GO 
