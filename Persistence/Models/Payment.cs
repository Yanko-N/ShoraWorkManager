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

        public DateTime PayedAt { get; set; } = DateTime.Now;
    }
}
