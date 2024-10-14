using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckMeInService.Models;

public class OfferedSubscriptions
{
    public OfferedSubscriptions()
    {
        SubscriptionId = Guid.NewGuid();
    }

    public OfferedSubscriptions(Guid? subscriptionId, string subscriptionName)
    {
        SubscriptionId = subscriptionId ?? Guid.NewGuid();
        SubscriptionName = subscriptionName;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("SubscriptionID")]
    public Guid SubscriptionId { get; set; }

    [Required] [MaxLength(128)] public required string SubscriptionName { get; set; }
}