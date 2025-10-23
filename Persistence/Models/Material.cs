using System.ComponentModel.DataAnnotations;

namespace Persistence.Models
{
    public class Material
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please insert the material name"),
            StringLength(100, MinimumLength = 3, ErrorMessage = "Must be between 3 and 100 characters")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "It's needed to have an available quantity for the material")]
        public float AvailableQuantity { get; set; } = 0;
    }
}
