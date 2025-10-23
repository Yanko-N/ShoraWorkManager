using System.ComponentModel.DataAnnotations;

namespace ShoraWorkManager.Models
{
    public class AccountIndexViewModel
    {
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
    }
}
