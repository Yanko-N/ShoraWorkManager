using System.ComponentModel.DataAnnotations;

namespace Persistence.Models
{
    public class Worker
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please insert the workers name"),
            StringLength(100, MinimumLength = 3, ErrorMessage = "Must be between 3 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "It's needed a value for the Worker")]
        [Range(0, float.MaxValue, ErrorMessage = "The price per hour must be a positive value")]
        public float PricePerHour { get; set; }

    }
}
