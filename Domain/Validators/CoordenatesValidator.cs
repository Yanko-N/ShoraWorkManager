using Domain.Exceptions;
using Domain.Validators.Interfaces;

namespace Domain.Validators
{
    public class CoordenatesValidator : IDataValidator<CoordenatesValidator.CoordenatesInput>
    {
        public class CoordenatesInput
        {
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
        }

        public void Validate(CoordenatesInput input)
        {
            List<string> errors = new List<string>();

            if (input.Latitude != null)
            {
                if (input.Latitude < -90 || input.Latitude > 90)
                {
                    errors.Add("Latitude must be between -90 and 90.");
                }
            }

            if (input.Longitude != null)
            {
                if (input.Longitude < -180 || input.Longitude > 180)
                {
                    errors.Add("Longitude must be between -180 and 180.");
                }
            }

            if (errors.Count > 0)
            {
                throw new CustomValidationException(errors);
            }
        }
    }
}
