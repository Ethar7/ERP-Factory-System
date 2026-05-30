namespace ErpFactory.Api.Contracts;

public record ApiResponse(
    bool Success,
    string Message,
    IReadOnlyCollection<string>? Errors = null)
{
    public static ApiResponse Ok(string message = "Operation completed successfully.") =>
        new(true, message);

    public static ApiResponse Fail(string message, IReadOnlyCollection<string>? errors = null) =>
        new(false, message, errors);
}

public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data = default,
    IReadOnlyCollection<string>? Errors = null)
    : ApiResponse(Success, Message, Errors)
{
    public static ApiResponse<T> Ok(T data, string message = "Operation completed successfully.") =>
        new(true, message, data);
}

public sealed record IdResponse(int Id);