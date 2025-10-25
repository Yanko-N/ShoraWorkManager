using System.ComponentModel.DataAnnotations;

namespace ShoraWorkManager.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please insert your first name"),
         MinLength(3, ErrorMessage = "Min length is 3"),
         MaxLength(100, ErrorMessage = "Max length is 100")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please insert your last name"),
         MinLength(3, ErrorMessage = "Min length is 3"),
         MaxLength(100, ErrorMessage = "Max length is 100")]
        public string LastName { get; set; }

        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Authorization token is required")]
        [Display(Name = "Authorization Token")]
        [StringLength(128, MinimumLength = 20, ErrorMessage = "Token length seems invalid.")]
        [RegularExpression(@"^[A-Za-z0-9_-]+$", ErrorMessage = "Only letters, numbers, '-' and '_' are allowed.")]
        public string AuthorizationToken { get; set; }

    }
}
