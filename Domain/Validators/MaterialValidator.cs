using Domain.Exceptions;
using Domain.Validators.Interfaces;

namespace Domain.Validators
{
    public class MaterialValidator : IDataValidator<MaterialValidator.MaterialInput>
    {
        public class MaterialInput
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public float AvailableQuantity { get; set; }
        }

        public void Validate(MaterialInput input)
        {
            IDataValidator<string> nameValidator = new NameValidator();
            List<string> errors = new List<string>();
            try
            {
                nameValidator.Validate(input.Name);
            }
            catch (CustomValidationException ex)
            {
                errors.AddRange(ex.ErrorsMessage);
            }

            if (input.AvailableQuantity < 0)
            {
                errors.Add("Available quantity cannot be negative.");
            }

            if (errors.Count > 0)
            {
                throw new CustomValidationException(errors);
            }
        }
    }
}
