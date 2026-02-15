namespace UserPhoneApp.Exceptions;

/// <summary>
/// Thrown when a requested entity is not found.
/// </summary>
public class NotFoundException : BusinessException
{
    public NotFoundException(string message) : base(message) { }
}