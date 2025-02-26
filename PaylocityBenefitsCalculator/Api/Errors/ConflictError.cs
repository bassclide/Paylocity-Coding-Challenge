namespace Api.Errors;

public class ConflictError : Error
{
    protected ConflictError(string message) : base(message) { }
    public override ErrorReason Reason => ErrorReason.Conflict;
}