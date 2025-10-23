using Domain.Validators.Interfaces;
using System.Text.RegularExpressions;

namespace Domain.Validators
{
    public class UserNameValidator : IDataValidator<UserNameValidator.UserNameInput>
    {
        public class UserNameInput
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public void Validate(UserNameInput input)
        {
            List<string> errors = new List<string>();

            //Just letter's and apostrophes allowed
            //this goes by:
            //can have letters small or big and apostrafes then times I want
            var namePattern = new Regex(@"^[\p{L}']+$");

            if (string.IsNullOrWhiteSpace(input.FirstName))
            {
                errors.Add("First name is required.");
            }

            if (input.FirstName.Length > 100)
            {
                errors.Add("First name is too long.");
            }

            if (string.IsNullOrWhiteSpace(input.LastName))
            {
                errors.Add("Last name is required.");
            }
            else if (!namePattern.IsMatch(input.FirstName))
            {
                errors.Add("First name must contain only letters.");
            }

            if (input.LastName.Length > 100)
            {
                errors.Add("Last name is too long.");
            }
            else if (!namePattern.IsMatch(input.FirstName))
            {
                errors.Add("First name must contain only letters.");
            }

            if (errors.Count > 0)
            {
                throw new Exceptions.CustomValidationException(errors);
            }
        }
    }
}
