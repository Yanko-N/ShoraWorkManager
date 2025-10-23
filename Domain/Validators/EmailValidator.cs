using Domain.Validators.Interfaces;

namespace Domain.Validators
{
    public class EmailValidator : IDataValidator<string>
    {
        public void Validate(string email)
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add("Email is required.");
            }
            if (!email.Contains("@") || !email.Contains("."))
            {
                errors.Add("Email format is invalid.");
            }

            try
            {
                System.Net.Mail.MailAddress mailAddress = new System.Net.Mail.MailAddress(email);
            }
            catch (Exception ex)
            {

                errors.Add(ex?.Message ?? ex?.InnerException?.Message ?? "Email format is invalid");
            }

            if (errors.Count > 0)
            {
                throw new Exceptions.CustomValidationException(errors);
            }
        }

    }
}
