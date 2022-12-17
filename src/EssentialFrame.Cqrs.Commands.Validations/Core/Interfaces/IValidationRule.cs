namespace EssentialFrame.Cqrs.Commands.Validations.Core.Interfaces;

public interface IValidationRule
{
    bool IsValid { get; }

    string PropertyName { get; }

    string ErrorMessage { get; }
}

