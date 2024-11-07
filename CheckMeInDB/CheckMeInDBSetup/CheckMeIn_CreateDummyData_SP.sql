CREATE PROCEDURE InsertDummyData
@FirstName NVARCHAR(129),
@LastName NVARCHAR(129), 
@PhoneNumber NVARCHAR(15)
AS
DECLARE @SubscriberId UNIQUEIDENTIFIER
DECLARE @SubscriptionID UNIQUEIDENTIFIER
BEGIN
    BEGIN TRY
        INSERT INTO Subscribers (FirstName, LastName, PhoneNumber)
        VALUES(@FirstName, @LastName, @PhoneNumber);

        SELECT @SubscriberId = SubscriberID FROM Subscribers WHERE PhoneNumber = PhoneNumber;
        SELECT @SubscriptionID = SubscriptionID FROM OfferedSubscriptions WHERE SubscriptionName = 'Exercise';

        INSERT INTO ActiveSubscriptions (SubscriberID, SubscriptionID, SubscriptionStartDate, PhoneNumber) 
        VALUES (@SubscriberID, @SubscriptionID, GETDATE(), @PhoneNumber);
    END TRY
    BEGIN CATCH
        SELECT
            ERROR_NUMBER() AS ErrorNumber, 
            ERROR_MESSAGE() AS ErrorMessage;
    END CATCH
END;