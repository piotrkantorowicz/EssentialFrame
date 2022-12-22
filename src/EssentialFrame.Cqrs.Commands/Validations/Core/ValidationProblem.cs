using EssentialFrame.Cqrs.Commands.Validations.Core.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Validations.Core;

public sealed class ValidationProblem
{
    private ValidationProblem()
    {
    }

    internal ValidationProblem(string propertyName, string errorMessage)
    {
        PropertyName = propertyName;
        ErrorMessage = errorMessage;
    }

    internal ValidationProblem(IValidationRule validationRule)
    {
        PropertyName = validationRule.PropertyName;
        ErrorMessage = validationRule.ErrorMessage;
    }

    public string PropertyName { get; }

    public string ErrorMessage { get; }

    public override string ToString() => ErrorMessage;
}





