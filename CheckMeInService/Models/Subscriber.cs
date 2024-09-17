namespace CheckMeInService.Models;

// This will be the object for handling queries too 
public class Subscriber
{
    private readonly Guid _subscriberId = Guid.NewGuid();
    public string Name;
    public string PhoneNumber; 
    public List<string>? Subscriptions = null;
    
    public Subscriber(string name, string phoneNumber )
    {
        Name = name;
        PhoneNumber = phoneNumber;
    }
    
    public string ReturnSubscriberId => _subscriberId.ToString();

    public void AddNewSubscription(string subscriptionName)
    {
        // Eventually we will have a db layer that queries but we'll set it up locally for now 
        if (Subscriptions == null)
        {
            Subscriptions = new List<string>();
        }
        
        // Read app settings.json
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        // check if subscription is offered
        // Todo: We'll probably add some db interaction here, for now just read from appsettings.json
        var subscriptions = config.GetSection("SmsSubscription").Get<List<string>>();

        // check if subscription is offered using linq
        bool isOffered = subscriptions != null && subscriptions.Select(word=> word.ToLower()).Contains(subscriptionName.ToLower());
        
        if (isOffered)
        {
            Subscriptions.Add(subscriptionName);
        }
        else
        {
            throw new Exception("Subscription not offered");
        }
    }


    public void RemoveExistingSubscription(string subscriptionName)
    {
        //Todo: Again we'll probably add some db interaction here, for now just remove it from the user object
        
        
        
    }
}