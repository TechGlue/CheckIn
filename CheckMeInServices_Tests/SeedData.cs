using CheckMeInService.Models;

namespace CheckMeInServices_Tests;

public static class SeedData
{
    public static async void PopulateTestData(CheckMeInContext dbContext)
    {
        var offeredSubscriptions = new OfferedSubscriptions
        {
            SubscriptionId = new Guid("5fb7097c-335c-4d07-b4fd-000004e2d28c"),
            SubscriptionName = "Test Subscription"
        };
        
        dbContext.OfferedSubscriptions.Add(offeredSubscriptions);
        await dbContext.SaveChangesAsync();
    }
}