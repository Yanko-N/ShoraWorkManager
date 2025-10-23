using System.ComponentModel.DataAnnotations;

namespace ShoraWorkManager.Models
{
    public class ChangeEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }
    }
}
