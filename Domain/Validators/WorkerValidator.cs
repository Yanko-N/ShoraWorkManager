using Domain.Exceptions;
using Domain.Validators.Interfaces;

namespace Domain.Validators
{
    public class WorkerValidator : IDataValidator<WorkerValidator.WorkerInput>
    {
        public class WorkerInput
        {
            public string Name { get; set; }
            public float PricePerHour { get; set; }
        }

        public void Validate(WorkerInput input)
        {
            IDataValidator<string> nameValidator = new MaterialNameValidator();
            List<string> errors = new List<string>();
            try
            {
                nameValidator.Validate(input.Name);
            }
            catch (CustomValidationException ex)
            {
                errors.AddRange(ex.ErrorsMessage);
            }

            if (input.PricePerHour <= 0)
            {
                errors.Add("Pricer per Hour must be greater than 0.");
            }

            if (errors.Count > 0)
            {
                throw new CustomValidationException(errors);
            }
        }
    }
}
