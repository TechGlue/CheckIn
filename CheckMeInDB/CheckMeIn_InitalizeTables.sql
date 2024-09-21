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
	SubscriptionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	SubscriptionName NVARCHAR(128) NOT NULL
)

-- Create the ActiveSubscriptions tables
CREATE TABLE ActiveSubscriptions
(
	ActiveSubscriptionID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
	SubscriberID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Subscribers(SubscriberID),
	SubscriptionID UNIQUEIDENTIFIER FOREIGN KEY REFERENCES OfferedSubscriptions(SubscriptionID),
	SubscriptionStartDate DATETIME NOT NULL,
	PhoneNumber NVARCHAR(15) NOT NULL,
)

