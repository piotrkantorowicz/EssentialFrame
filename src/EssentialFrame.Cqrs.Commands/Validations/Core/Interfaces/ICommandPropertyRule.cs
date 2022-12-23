namespace EssentialFrame.Cqrs.Commands.Validations.Core.Interfaces;

internal interface ICommandPropertyRule<in TCommand>
{
    ValidationProblem Validate(TCommand commandContext);
}
