namespace Domain.Exceptions
{
    /// <summary>
    /// Represents one or more validation failures.
    /// </summary>
    public class CustomValidationException : Exception
    {
        public IReadOnlyList<string> ErrorsMessage { get; }
        private bool _isUniqueString = true;

        public CustomValidationException(string message)
            : base(message)
        {
            ErrorsMessage = new List<string>()
            {
                message
            };
        }

        public CustomValidationException(IEnumerable<string> errors)
            : base("One or more validation errors occurred.")
        {

            _isUniqueString = false;
            ErrorsMessage = errors.ToList();
        }

        public override string ToString()
        {
            if (_isUniqueString)
            {
                return base.ToString();
            }

            var details = string.Join(Environment.NewLine, ErrorsMessage);
            return $"{Message}{Environment.NewLine}{details}";
        }
    }
}
