using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckMeInService.Models;

public class Subscriber
{
    public Subscriber()
    {
        SubscriberId = Guid.NewGuid();
    }

    public Subscriber(Guid? subscriberId, string firstName, string lastName, string phoneNumber)
    {
        SubscriberId = subscriberId ?? Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("SubscriberID")]
    public Guid SubscriberId { get; set; }

    [Required] [MaxLength(128)] public required string FirstName { get; set; }

    [Required] [MaxLength(128)] public required string LastName { get; set; }

    [Required]
    [MaxLength(128)]
    [Index(IsUnique = true)]
    public required string PhoneNumber { get; set; }
}