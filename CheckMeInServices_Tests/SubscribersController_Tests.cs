using System.Net;
using System.Net.Http.Json;
using CheckMeInService.Models;
using CheckMeInService.Repository;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.SqlEdge;

namespace CheckMeInServices_Tests;

public class SubscribersControllerTests : IClassFixture<CustomWebApplicationFactory>, IClassFixture<TestDatabaseFixture>
{
    private readonly HttpClient _client;

    public SubscribersControllerTests(TestDatabaseFixture dbFixture)
    {
        var factory = new CustomWebApplicationFactory(dbFixture._container.GetConnectionString());
        _client = factory.CreateClient();
    }


    [Fact]
    public async Task TestSubscribers_ReturnsOK()
    {
        // create a mock client of our application  
        var response = await _client.GetAsync($"/v1/subscribers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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