namespace UserPhoneApp.Exceptions;

/// <summary>
/// Base exception type for business logic errors.
/// </summary>
public abstract class BusinessException : Exception
{
    protected BusinessException(string message) : base(message) { }
}