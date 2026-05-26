namespace ErpFactory.Api.Contracts;

public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data = default,
    IReadOnlyCollection<string>? Errors = null)
{
    public static ApiResponse<T> Ok(T data, string message = "Operation completed successfully") =>
        new(true, message, data);

    public static ApiResponse<T> Fail(string message, IReadOnlyCollection<string>? errors = null) =>
        new(false, message, default, errors);
}

public sealed record IdResponse(int Id);
