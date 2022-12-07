namespace EssentialFrame.Cqrs.Validations.Core.Interfaces;

internal interface ICommandPropertyRule<in TCommand>
{
    ValidationProblem Validate(TCommand commandContext);
}
