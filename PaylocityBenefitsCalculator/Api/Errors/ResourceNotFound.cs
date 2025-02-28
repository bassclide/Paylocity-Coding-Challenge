namespace Api.Errors;

public class ResourceNotFound : Error
{
    protected ResourceNotFound(string message) : base(message)
    {
    }

    public override ErrorReason Reason => ErrorReason.NotFound;
}