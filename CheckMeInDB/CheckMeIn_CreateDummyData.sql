-- Todo Create a stored procedure that fetches this data that is returned from the first two queries 

INSERT INTO Subscribers (FirstName, LastName, PhoneNumber)
VALUES('FirstTester', 'LastTester', '111-111-1111');


INSERT INTO OfferedSubscriptions (SubscriptionName)
VALUES('Exercise');


INSERT INTO ActiveSubscriptions (SubscriberID, SubscriptionID, SubscriptionStartDate, PhoneNumber)
VALUES('f78ad9cc-e248-4252-9bb1-7368dc8b798e', 'e8852d2f-38ff-4c89-a063-7154a4b557cc', GETDATE(), '111-111-1111');



