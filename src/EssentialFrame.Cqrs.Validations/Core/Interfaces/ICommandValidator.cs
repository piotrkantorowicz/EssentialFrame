namespace EssentialFrame.Cqrs.Validations.Core.Interfaces;

public interface ICommandValidator<in TCommand>
{
    ValidationResult Validate(TCommand command);
}
