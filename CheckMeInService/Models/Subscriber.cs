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
        // Evantually we will have a db that queries but we'll wee
        
        
        
    } 
}