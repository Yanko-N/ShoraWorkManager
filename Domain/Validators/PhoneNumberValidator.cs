using Domain.Validators.Interfaces;
using System.Text.RegularExpressions;

namespace Domain.Validators
{
    public class PhoneNumberValidator : IDataValidator<string>
    {
        public void Validate(string input)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(input))
            {
                errors.Add("Phone number is required.");
            }
            else
            {
                // Portuguese number:  9 numbers
                var pattern = new Regex(@"^\d{9}$");

                if (!pattern.IsMatch(input))
                {
                    errors.Add("Phone number must be a Portuguese one");
                }
            }

            if (errors.Count > 0)
            {
                throw new Exceptions.CustomValidationException(errors);
            }
        }
    }
}
