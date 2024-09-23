namespace CheckMeInService.Models;

public class Subscriber
{
    private readonly Guid _subscriberId;
    public string FirstName;
    public string LastName;
    public string PhoneNumber; 
    public List<string>? Subscriptions = null;
    
    public Subscriber(Guid? guid, string firstName, string lastName, string phoneNumber )
    {
        if (guid is null)
        {
            _subscriberId = Guid.NewGuid();
        }
        else
        {
            _subscriberId = (Guid)guid;
        }

        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }
    
    public string ReturnSubscriberId => _subscriberId.ToString();

    public void AddNewSubscription(string subscriptionName)
    {
        // Eventually we will have a db layer that queries but we'll set it up locally for now 
        Subscriptions ??= new List<string>();
        
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
        // if subscription is null instanciate
        Subscriptions ??= new List<string>();

        if (Subscriptions.Contains(subscriptionName))
        {
            Subscriptions.Remove(subscriptionName);
        }
        else
        {
            throw new Exception($"Subscription not found for {FirstName} {LastName}");
        }
    }
}

