namespace Shortly.API.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; }

        protected Result(bool isSuccess, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, error);
    }
    public class Result<TValue> : Result
    {
        public TValue? Value { get; }

        protected Result(TValue? value, bool isSuccess, string? error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<TValue> Success(TValue value) => new(value, true, null);
        public static new Result<TValue> Failure(string error) => new(default, false, error);
    }
}