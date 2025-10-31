using Domain.Exceptions;
using Domain.Validators.Interfaces;

namespace Domain.Validators
{
    public class ConstructionSiteValidator : IDataValidator<ConstructionSiteValidator.ConstructionSiteInput>
    {
        public class ConstructionSiteInput
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
            public int ClientId { get; set; }
        }

        public void Validate(ConstructionSiteInput input)
        {
            IDataValidator<string> nameValidator = new NameValidator();
            IDataValidator<CoordenatesValidator.CoordenatesInput> coordenatesValidator = new CoordenatesValidator();

            List<string> errors = new List<string>();
            try
            {
                nameValidator.Validate(input.Name);
            }
            catch (CustomValidationException ex)
            {
                errors.AddRange(ex.ErrorsMessage);
            }

            if(input.ClientId <= 0)
            {
                errors.Add("ClientId must be a positive integer.");
            }

            try
            {
                coordenatesValidator.Validate(new CoordenatesValidator.CoordenatesInput()
                {
                    Latitude = input.Latitude,
                    Longitude = input.Longitude
                });
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
