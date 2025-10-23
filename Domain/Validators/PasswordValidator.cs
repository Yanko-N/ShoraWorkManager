using Domain.Exceptions;
using Domain.Validators.Interfaces;
using System.Text.RegularExpressions;

namespace Domain.Validators
{
    public class PasswordValidator : IDataValidator<string>
    {
        public void Validate(string password)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Password is required.");
            }
            else
            {
                if (password.Length < 8){
                    errors.Add("Password must be at least 8 characters long.");
                }

                if (!password.Any(char.IsUpper)){
                    errors.Add("Password must contain at least one uppercase letter.");
                }

                if (!password.Any(char.IsLower)){
                    errors.Add("Password must contain at least one lowercase letter.");
                }

                if (!password.Any(char.IsDigit)){
                    errors.Add("Password must contain at least one number.");
                }

                if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':{}|<>_\-+=\\/\[\]]")){
                    errors.Add("Password must contain at least one special character.");
                }
            }

            if (errors.Count > 0)
            {
                throw new CustomValidationException(errors);
            }
        }
    }
}
