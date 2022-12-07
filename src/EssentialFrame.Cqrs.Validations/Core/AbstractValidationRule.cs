using EssentialFrame.Cqrs.Validations.Core.Interfaces;

namespace EssentialFrame.Cqrs.Validations.Core;

public abstract class AbstractValidationRule : IValidationRule
{
    protected AbstractValidationRule(string propertyName) => PropertyName = propertyName;

    public abstract bool IsValid { get; }

    public abstract string ErrorMessage { get; }

    public string PropertyName { get; protected init; }
}
