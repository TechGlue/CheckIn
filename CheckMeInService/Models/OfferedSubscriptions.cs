using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckMeInService.Models;

public class OfferedSubscriptions
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("SubscriptionID")]
    public Guid SubscriptionId { get; set; }

    [Required] [MaxLength(128)] public string SubscriptionName { get; set; }
    public OfferedSubscriptions()
    {
    }

    public OfferedSubscriptions(Guid? subscriptionId, string subscriptionName)
    {
        SubscriptionId = subscriptionId ?? Guid.NewGuid();
        SubscriptionName = subscriptionName;
    }
}