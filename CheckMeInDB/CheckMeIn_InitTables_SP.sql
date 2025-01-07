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
	    PhoneNumber NVARCHAR(128) NOT NULL UNIQUE
	  )
	  
	  -- Create a table to hold the subscriptions that are offered. For now it's just going to be exercise 
	  CREATE TABLE OfferedSubscriptions
	  (
	  	SubscriptionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID() NOT NULL,
	  	SubscriptionName NVARCHAR(128) NOT NULL UNIQUE
	  )
	  
	  -- Create the ActiveSubscriptions tables
	  CREATE TABLE ActiveSubscriptions
	  (
	  	ActiveSubscriptionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID() NOT NULL,
	  	SubscriberID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Subscribers(SubscriberID) NOT NULL,
	  	SubscriptionID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES OfferedSubscriptions(SubscriptionID) NOT NULL,
	  	SubscriptionStartDate DATETIME NOT NULL,
	  	PhoneNumber NVARCHAR(128),
	  )

	  -- Create the CheckIn table
	  CREATE TABLE CheckIn
	  (
	  	CheckInID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID() NOT NULL,
	  	ActiveSubscriptionsID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES ActiveSubscriptions(ActiveSubscriptionID) NOT NULL,
	  	LastCheckInDate DATETIME NOT NULL,
	  	FutureCheckInDate DATETIME NOT NULL,
		TotalCheckIns INT NOT NULL,
	  )

	  -- Create the CheckIn history table
	  CREATE TABLE CheckInHistory
	  (
		-- Make the checkin id non guid make it an incremental int we don't need to query this table by id
	  	CheckInHistoryID INT PRIMARY KEY IDENTITY(1,1) NOT NULL, -- start at 1 and increase by 1 
		SubscriptionID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES OfferedSubscriptions(SubscriptionID) NOT NULL,
		SubscriberID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Subscribers(SubscriberID) NOT NULL,
		CheckInDate DATETIME NOT NULL,
		SubscriptionName NVARCHAR(128) NOT NULL,
	  )

	  -- Load the default OfferedSubscription
	  INSERT INTO OfferedSubscriptions ( SubscriptionID, SubscriptionName)
	  VALUES ('abcdef12-3456-7890-abcd-ef1234567890', 'Exercise')

	  INSERT INTO Subscribers (SubscriberID, FirstName, LastName, PhoneNumber)
	  VALUES ('12345678-1234-1234-1234-123456789abc', 'testerFirstName', 'testerLastName', '555-555-5555');

	  INSERT INTO ActiveSubscriptions 
	  (ActiveSubscriptionID, SubscriberID, SubscriptionID, SubscriptionStartDate, PhoneNumber) 
	  VALUES 
	  ('abcdef12-3456-7890-abcd-ef1234561111', '12345678-1234-1234-1234-123456789abc', 'abcdef12-3456-7890-abcd-ef1234567890', '2021-01-01', '555-555-5555');
	  
    END TRY
    BEGIN CATCH
        SELECT
            ERROR_NUMBER() AS ErrorNumber, 
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END;
GO 
