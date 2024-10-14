using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckMeInService.Models;

public class CheckIn
{
    public CheckIn()
    {
        CheckInId = Guid.NewGuid();
    }

    public CheckIn(Guid? checkInId, Guid activeSubscriptionId, DateTime lastCheckInDate, DateTime futureCheckInDate,
        int currentStreak)
    {
        CheckInId = checkInId ?? Guid.NewGuid();
        ActiveSubscriptionId = activeSubscriptionId;
        LastCheckInDate = lastCheckInDate;
        FutureCheckInDate = futureCheckInDate;
        TotalCheckIns = currentStreak;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("CheckInID")]
    public Guid CheckInId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ActiveSubscriptionsID")]
    public Guid ActiveSubscriptionId { get; set; }

    [Column("LastCheckInDate")] public DateTime LastCheckInDate { get; set; }

    [Column("FutureCheckInDate")] public DateTime FutureCheckInDate { get; set; }

    [Column("TotalCheckIns")] public int TotalCheckIns { get; set; }
}