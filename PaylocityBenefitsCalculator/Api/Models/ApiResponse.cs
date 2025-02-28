namespace Api.Models;

public sealed class ApiResponse<T>
{
    public T? Data { get; init; }
    public bool Success { get; init; } = true;
    public string Message { get; init; } = string.Empty;
    public string Error { get; init; } = string.Empty;
}
