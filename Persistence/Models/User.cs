using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Persistence.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Please insert your first name"),
            MinLength(3, ErrorMessage = "Min lenght is 3"),
           MaxLength(100, ErrorMessage = "Max lenght is 100")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please insert your last name"),
            MinLength(3, ErrorMessage = "Min lenght is 3"),
            MaxLength(100,ErrorMessage ="Max lenght is 100")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "It's required to have an email associated on the account"),
            DataType(DataType.EmailAddress, ErrorMessage = "Invalid email, please insert a real email")]
        public override string Email
        {
            get => base.Email;
            set => base.Email = value;
        }

        public string GetUserName()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
