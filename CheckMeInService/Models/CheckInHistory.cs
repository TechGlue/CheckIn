using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckMeInService.Models;

public class CheckInHistory
{
    public CheckInHistory(Guid subscriptionId, Guid subscriberId, DateTime lastCheckInDate,
        string subscriptionName)
    {
        SubscriptionId = subscriptionId;
        SubscriberId = subscriberId;
        LastCheckInDate = lastCheckInDate;
        SubscriptionName = subscriptionName;
    }

    [Key] [Column("CheckInHistoryID")] public int CheckInHistoryId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("SubscriptionID")]
    public Guid SubscriptionId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("SubscriberID")]
    public Guid SubscriberId { get; set; }

    [Column("CheckInDate")] public DateTime LastCheckInDate { get; set; }

    [Column("SubscriptionName")] public string SubscriptionName { get; set; }
}