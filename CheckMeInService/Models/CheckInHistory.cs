using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckMeInService.Models;

public class CheckInHistory
{
    public CheckInHistory(int checkInHistoryId, Guid subscriptionId, Guid subscriberId, DateTime lastCheckInDate,
        string subscriptionName)
    {
        CheckInHistoryId = checkInHistoryId;
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

    [Column("LastCheckInDate")] public DateTime LastCheckInDate { get; set; }

    [Column("SubscriptionName")] public string SubscriptionName { get; set; }
}