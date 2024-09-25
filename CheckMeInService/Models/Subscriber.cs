using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckMeInService.Models;

public class Subscriber
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("SubscriberID")]
    public Guid SubscriberId { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string FirstName { get; set; }
    
    [Required]
    [MaxLength(128)]
    public string LastName { get; set; }
    
    [Required]
    [MaxLength(15)]
    [Index(IsUnique = true)]
    public string PhoneNumber { get; set; }
    
    public List<string>? Subscriptions = null;

    public Subscriber()
    {
    }
    public Subscriber(Guid? subscriberId, string firstName, string lastName, string phoneNumber )
    {
        SubscriberId = subscriberId ?? Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }

    public string ReturnSubscriberId => SubscriberId.ToString();

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

