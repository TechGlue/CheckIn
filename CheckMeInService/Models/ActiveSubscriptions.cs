using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckMeInService.Models;

public class ActiveSubscriptions
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ActiveSubscriptionID")]
    public Guid ActiveSubscriptionId { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("SubscriberID")]
    public Guid SubscriberId { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("SubscriptionID")]
    public Guid SubscriptionId { get; set; }
    
    [Column("SubscriptionStartDate")]
    public DateTime SubscriptionStartDate { get; set; }

    [MaxLength(15)]
    public string? PhoneNumber { get; set; }
    
    public ActiveSubscriptions()
    {
    }
    public ActiveSubscriptions(Guid? activeSubscriptionId, Guid subscriberId, Guid subscriptionId, DateTime subscriptionStartDate, string? phoneNumber)
    {
        ActiveSubscriptionId = activeSubscriptionId ?? Guid.NewGuid();
        SubscriberId = subscriberId; 
        SubscriptionId = subscriptionId;
        SubscriptionStartDate = subscriptionStartDate;
        PhoneNumber = phoneNumber;
    }
}