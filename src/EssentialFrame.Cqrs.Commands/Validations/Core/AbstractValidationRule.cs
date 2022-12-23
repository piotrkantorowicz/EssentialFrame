using EssentialFrame.Cqrs.Commands.Validations.Core.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Validations.Core;

public abstract class AbstractValidationRule : IValidationRule
{
    protected AbstractValidationRule(string propertyName) => PropertyName = propertyName;

    public abstract bool IsValid { get; }

    public abstract string ErrorMessage { get; }

    public string PropertyName { get; protected init; }
}
