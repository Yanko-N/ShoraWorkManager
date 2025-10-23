namespace Domain.Validators.Interfaces
{
    /// <summary>Validates an input object against domain rules.</summary>
    /// <typeparam name="TInput">The type to validate.</typeparam>
    public interface IDataValidator<TInput> 
    {
        /// <summary>
        /// This method will validate the input data.
        /// </summary>
        /// <param name="input">object to be validated</param>
        /// <exception cref="Domain.Exceptions.CustomValidationException">
        /// Thrown when the object fails validation. Inspect <see cref="Domain.Exceptions.CustomValidationException.ErrorsMessage"/> for details.
        /// </exception>
        void Validate(TInput input);
    }
}
