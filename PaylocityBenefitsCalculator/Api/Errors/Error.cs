namespace Api.Errors;

public abstract class Error(string message)
{
    public abstract ErrorReason Reason { get; }

    public string Message { get; } = message;
}