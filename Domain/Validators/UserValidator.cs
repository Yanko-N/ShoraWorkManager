using Domain.Exceptions;
using Domain.Validators.Interfaces;

namespace Domain.Validators
{
    public class UserValidator : IDataValidator<UserValidator.UserValidatorInput>
    {
        public class UserValidatorInput 
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public void Validate(UserValidatorInput input)
        {
            var errors = new List<string>();

            IDataValidator<UserNameValidator.UserNameInput> nameValidator = new UserNameValidator();
            try
            {
                nameValidator.Validate(new UserNameValidator.UserNameInput
                {
                    FirstName = input.FirstName,
                    LastName = input.LastName
                });
            }
            catch (CustomValidationException ex)
            {
                errors.AddRange(ex.ErrorsMessage);
            }

            IDataValidator<string> emailValidator = new EmailValidator();
            try
            {
                emailValidator.Validate(input.Email);
            }
            catch (CustomValidationException ex)
            {
                errors.AddRange(ex.ErrorsMessage);
            }

            IDataValidator<string> passwordValidator = new PasswordValidator();
            try
            {
                passwordValidator.Validate(input.Password);
            }
            catch (CustomValidationException ex)
            {
                errors.AddRange(ex.ErrorsMessage);
            }

            if (errors.Count > 0)
            {
                throw new CustomValidationException(errors);
            }
        }
    }
}
