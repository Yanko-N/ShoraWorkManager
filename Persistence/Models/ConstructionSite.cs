using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Models
{
    public class ConstructionSite
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please insert the construction site name"),
            StringLength(100, MinimumLength = 3, ErrorMessage = "Must be between 3 and 100 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [Required(ErrorMessage ="It's needed a state for the construction site")]
        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(Owner))]
        public int ClientId { get; set; }
        public virtual Client Owner { get; set; }
    }
}
