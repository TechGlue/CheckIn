using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckMeInService.Models;

public class CheckIn
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("CheckInID")]
    public Guid CheckInId { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("ActiveSubscriptionsID")]
    public Guid ActiveSubscriptionId { get; set; }
    
    [Column("LastCheckInDate")]
    public DateTime LastCheckInDate { get; set; }
    
    [Column("FutureCheckInDate")]
    public DateTime FutureCheckInDate { get; set; }
    
    [Column("CurrentStreak")]
    public int CurrentStreak { get; set; }
    
    public CheckIn()
    {
    }
    public CheckIn(Guid? checkInId, Guid activeSubscriptionId, DateTime lastCheckInDate, DateTime futureCheckInDate, int currentStreak)
    {
        CheckInId = checkInId ?? Guid.NewGuid();
        ActiveSubscriptionId = activeSubscriptionId;
        LastCheckInDate = lastCheckInDate;
        FutureCheckInDate = futureCheckInDate;
        CurrentStreak = currentStreak;
    }
}