namespace AuthService.Common;

public readonly record struct Result
{
    public bool IsSuccess { get; init; }
    public ServiceFailure? Failure { get; init; }

    public static Result Success() => new() { IsSuccess = true };
    public static Result Fail(ServiceFailure failure) => new() { IsSuccess = false, Failure = failure };
}