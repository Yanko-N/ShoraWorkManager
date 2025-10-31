using Domain.Exceptions;
using Domain.Validators.Interfaces;

namespace Domain.Validators
{
    public class ClientValidator : IDataValidator<ClientValidator.ClientInput>
    {
        public class ClientInput
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
        }

        public void Validate(ClientInput input)
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

            IDataValidator<string> phoneValidator = new PhoneNumberValidator();
            try
            {
                phoneValidator.Validate(input.PhoneNumber);
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
