using System.Net;

namespace CheckMeInServices_Tests;

public class SubscribersController_Tests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SubscribersController_Tests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task TestSubscribersController_ReturnsOK()
    {
        // create a mock client of our application  
        HttpResponseMessage response = await _client.GetAsync(new Uri($"/v1/subscribers"));
        string content = await response.Content.ReadAsStringAsync();
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("\"Subscribers Controllers is working\"", content);
    }

    [Fact]
    public async Task CreateActiveSubscription_ReturnsNotFound_InvalidSubscription()
    {
        // Arrange 
        string firstName = "luis";
        string lastName = "garcia";
        string phoneNumber = "404-404-1111";
        string subscriptionName = "InvalidSubscription";
        // can tests auth here when role based auth is implemented 
        var response =
            await _client.GetAsync(
                $"v1/subscribers/?firstName={firstName}&lastName={lastName}&phoneNumber={phoneNumber}&subscriptionName={subscriptionName}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Subscription is currently not being offered", response.Content.ToString());
    }

    [Fact]
    public async Task CreateActiveSubscription_ReturnsOK()
    {
        // Arrange 
        string firstName = "luis";
        string lastName = "garcia";
        string phoneNumber = "404-404-1111";
        string subscriptionName = "TestingInCSharp";
        // can tests auth here when role based auth is implemented 
        var response =
            await _client.GetAsync(
                $"v1/subscribers/?firstName={firstName}&lastName={lastName}&phoneNumber={phoneNumber}&subscriptionName={subscriptionName}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}