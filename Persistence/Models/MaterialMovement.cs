using System.ComponentModel.DataAnnotations;

namespace Persistence.Models
{
    public class MaterialMovement
    {
        [Key]
        public int Key { get; set; }

        [Required(ErrorMessage = "Please insert the material")]
        public int MaterialId { get; set; }
        public virtual Material Material { get; set; }

        [Required(ErrorMessage = "Please insert the Construction Site")]
        public int ConstructionSiteId { get; set; }
        public virtual ConstructionSite ConstructionSite { get; set; }

        [Required(ErrorMessage = "Please insert the quantity moved")]
        public float Quantity { get; set; }

        public DateTime MovementDate { get; set; } = DateTime.Now;

    }
}
