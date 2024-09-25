using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckMeInService.Models;

public class Subscriptions
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ActiveSubscriptionID")]
    public Guid ActiveSubscriptionId { get; set; }
    
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("SubscriberID")]
    public Guid SubscriberId { get; set; }
    
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("SubscriptionID")]
    public Guid SubscriptionId { get; set; }
    
    [Column("SubscriptionStartDate")]
    public DateTime SubscriptionStartDate { get; set; }

    [MaxLength(15)]
    public string PhoneNumber { get; set; }
    
    public Subscriptions()
    {
    }
    public Subscriptions(Guid? activeSubscriptionId, Guid? subscriberId, Guid? subscriptionId, DateTime subscriptionStartDate, string phoneNumber)
    {
        ActiveSubscriptionId = activeSubscriptionId ?? Guid.NewGuid();
        SubscriberId = subscriberId ?? Guid.NewGuid(); 
        SubscriptionId = subscriptionId ?? Guid.NewGuid();
        SubscriptionStartDate = subscriptionStartDate;
        PhoneNumber = phoneNumber;
    }
}