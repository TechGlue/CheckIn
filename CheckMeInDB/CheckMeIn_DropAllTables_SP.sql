DROP PROCEDURE IF EXISTS dbo.DropAllTables
GO
CREATE PROCEDURE dbo.DropAllTables 
AS
BEGIN
    BEGIN TRY
	  DROP TABLE dbo.ActiveSubscriptions;
	  DROP TABLE dbo.Subscribers;
	  DROP TABLE dbo.OfferedSubscriptions;
	  DROP TABLE dbo.CheckIn;
    END TRY
    BEGIN CATCH
        SELECT
            ERROR_NUMBER() AS ErrorNumber, 
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END;
GO 
