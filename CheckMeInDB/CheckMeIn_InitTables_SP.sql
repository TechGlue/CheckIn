DROP PROCEDURE IF EXISTS dbo.InitTables
GO
CREATE PROCEDURE dbo.InitTables 
AS
BEGIN
    BEGIN TRY
	  -- Create the Subscribers table
	  CREATE TABLE Subscribers
	  (
	    SubscriberID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	    FirstName NVARCHAR(128) NOT NULL,
	    LastName NVARCHAR(128) NOT NULL,
	    PhoneNumber NVARCHAR(15) NOT NULL UNIQUE
	  )
	  
	  -- Create a table to hold the subscriptions that are offered. For now it's just going to be exercise 
	  CREATE TABLE OfferedSubscriptions
	  (
	  	SubscriptionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID() NOT NULL,
	  	SubscriptionName NVARCHAR(128) NOT NULL
	  )
	  
	  -- Create the ActiveSubscriptions tables
	  CREATE TABLE ActiveSubscriptions
	  (
	  	ActiveSubscriptionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID() NOT NULL,
	  	SubscriberID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Subscribers(SubscriberID) NOT NULL,
	  	SubscriptionID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES OfferedSubscriptions(SubscriptionID) NOT NULL,
	  	SubscriptionStartDate DATETIME NOT NULL,
	  	PhoneNumber NVARCHAR(15) NOT NULL UNIQUE,
	  )

	  -- Create the CheckIn table
	  CREATE TABLE CheckIn
	  (
	  	CheckInID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID() NOT NULL,
	  	ActiveSubscriptionsID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES ActiveSubscriptions(ActiveSubscriptionID) NOT NULL,
	  	LastCheckInDate DATETIME NOT NULL,
	  	FutureCheckInDate DATETIME NOT NULL,
		CurrentStreak INT NOT NULL,
	  )

	  -- Load the default OfferedSubscription
	  INSERT INTO OfferedSubscriptions (SubscriptionName)
	  VALUES ('Exercise')

    END TRY
    BEGIN CATCH
        SELECT
            ERROR_NUMBER() AS ErrorNumber, 
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END;
GO 
