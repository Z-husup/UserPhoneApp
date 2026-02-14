namespace UserPhoneApp.Exceptions;

public class ValidationException : BusinessException
{
    public string PropertyName { get; }

    public ValidationException(string propertyName, string message)
        : base(message)
    {
        PropertyName = propertyName;
    }
}