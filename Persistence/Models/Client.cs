using System.ComponentModel.DataAnnotations;

namespace Persistence.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Please insert your first name"),
            Length(3,100,ErrorMessage = "Must be between 3 and 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please insert your last name"),
            Length(3, 100, ErrorMessage = "Must be between 3 and 100 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="It's required to have an email associated on the account"),
            DataType(DataType.EmailAddress,ErrorMessage ="Invalid email, please insert a real email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please insert your phone number"), 
            DataType(DataType.PhoneNumber, ErrorMessage = "Invalid phone number, please insert a real phone number")]
        public string PhoneNumber { get; set; }


    }
}
