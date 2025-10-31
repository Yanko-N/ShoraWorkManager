using Domain.Exceptions;
using Domain.Validators.Interfaces;

namespace Domain.Validators
{
    internal class NameValidator : IDataValidator<string>
    {
        public void Validate(string input)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(input))
            {
                throw new CustomValidationException("Material name cannot be empty.");
            }

            if (input.Length < 3 || input.Length > 100)
            {
                errors.Add("Material name must be between 3 and 100 characters long.");
            }

            if (errors.Count > 0)
            {
                throw new Exceptions.CustomValidationException(errors);
            }
        }
    }
}
