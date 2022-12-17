namespace EssentialFrame.Cqrs.Commands.Validations.Core.Interfaces;

public interface ICommandValidator<in TCommand>
{
    ValidationResult Validate(TCommand command);
}

