namespace UserPhoneApp.Exceptions;

/// <summary>
/// Thrown when business validation rules are violated.
/// </summary>
public class ValidationException : BusinessException
{
    public string PropertyName { get; }

    public ValidationException(string propertyName, string message)
        : base(message)
    {
        PropertyName = propertyName;
    }
}