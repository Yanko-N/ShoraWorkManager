using System.ComponentModel.DataAnnotations;

namespace Persistence.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please insert the Worked hours of Construction Site")]
        public int ContructionSiteWorkedHoursWorkerId { get; set; }
        public virtual ContructionSiteWorkedHoursWorker ContructionSiteWorkedHoursWorker { get; set; }

        [Required(ErrorMessage ="It's needed a value for the payments")]
        public float Value { get; set; } = 0;
        public DateTime PayedAt { get; set; } = DateTime.Now;
    }
}
