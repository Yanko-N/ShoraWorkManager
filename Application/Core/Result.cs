namespace Application.Core
{
    public class Result<T>
    {
        public T? Value { get; }
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public IReadOnlyList<string> Errors { get; }

        protected Result(bool isSuccess, IEnumerable<string>? errors = null , T? value = default(T))
        {
            IsSuccess = isSuccess;
            Errors = errors?.ToList() ?? new List<string>();
            Value = value;
        }

        public static Result<T> Failure(params string[] errors) => new Result<T>(false, errors);

        public static Result<T> Failure(IEnumerable<string> errors) => new Result<T>(false, errors);

        public static Result<T> Success(T value) => new Result<T>(true,null, value);

        public override string ToString()
        {
            if (IsSuccess)
            {
                return Value?.ToString() ?? "Sucess";
            }

            return $"Failure: {string.Join("; ", Errors)}";
        }
    }
}
