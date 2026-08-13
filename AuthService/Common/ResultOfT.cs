namespace AuthService.Common;

public readonly record struct Result<T>
{
    public bool IsSuccess { get; init; }
    public ServiceFailure? Failure { get; init; }
    public T? Value { get; init; }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Fail(ServiceFailure failure) => new() { IsSuccess = false, Failure = failure };
}