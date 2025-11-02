using System.ComponentModel.DataAnnotations;

namespace Persistence.Models
{
    public class ContructionSiteWorkedHoursWorker
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please insert the Construction Site")]
        public int ConstructionSiteId { get; set; }
        public virtual ConstructionSite? ConstructionSite { get; set; }

        [Required(ErrorMessage = "Please insert the Worker")]
        public int WorkerId { get; set; }

        public virtual Worker? Worker { get; set; }

        [Required(ErrorMessage = "Please insert the number of worked hours")]
        public float WorkedHours { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.Now;

        public bool WasPayed { get; set; } = false;

    }
}
