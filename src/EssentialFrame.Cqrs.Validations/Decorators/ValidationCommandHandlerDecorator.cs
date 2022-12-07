using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Errors;
using EssentialFrame.Cqrs.Interfaces;
using EssentialFrame.Cqrs.Validations.Validators;

namespace EssentialFrame.Cqrs.Validations.Decorators;

public sealed class ValidationCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _decorated;
    private readonly IList<CommandAbstractValidator<TCommand>> _validators;

    internal ValidationCommandHandlerDecorator(IList<CommandAbstractValidator<TCommand>> validators,
                                               ICommandHandler<TCommand> decorated)
    {
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));

        _validators = validators;
    }

    public ICommandResult Handle(TCommand command)
    {
        try
        {
            var failures = _validators
                           .Select(x => x.Validate(command))
                           .Where(x => !x.IsValid)
                           .ToList();

            if (failures.Any())
            {
                var errors = failures
                             .SelectMany(x => x.Errors)
                             .Select(x => new KeyValuePair<string, string>(x.PropertyName, x.ErrorMessage))
                             .ToList();

                return CommandResult.Fail(new ValidationError(errors));
            }
        }
        catch (Exception ex)
        {
            return
                CommandResult.Fail(new
                                       UnexpectedError($"Unexpected exception occurred while validating command: {command.GetType().Name}",
                                                       ex));
        }

        return _decorated.Handle(command);
    }

    public async Task<ICommandResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var failures = _validators
                           .Select(x => x.Validate(command))
                           .Where(x => !x.IsValid)
                           .ToList();

            if (failures.Any())
            {
                var errors = failures
                             .SelectMany(x => x.Errors)
                             .Select(x => new KeyValuePair<string, string>(x.PropertyName, x.ErrorMessage))
                             .ToList();

                return CommandResult.Fail(new ValidationError(errors));
            }
        }
        catch (Exception ex)
        {
            return
                CommandResult
                    .Fail(new
                              UnexpectedError($"Unexpected exception occurred while validating command: {command.GetType().Name}",
                                              ex));
        }

        return await _decorated.HandleAsync(command, cancellationToken);
    }
}
