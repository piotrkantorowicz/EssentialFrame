using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Errors;
using EssentialFrame.Cqrs.Commands.Logging;
using EssentialFrame.Cqrs.Commands.Validations.Logging;
using EssentialFrame.Extensions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EssentialFrame.Cqrs.Commands.Validations.Decorators;

public sealed class ValidationCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _decorated;
    private readonly ILogger<ValidationCommandHandlerDecorator<TCommand>> _logger;
    private readonly IList<IValidator<TCommand>> _validators;

    internal ValidationCommandHandlerDecorator(IList<IValidator<TCommand>> validators,
        ICommandHandler<TCommand> decorated, ILogger<ValidationCommandHandlerDecorator<TCommand>> logger)
    {
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _validators = validators;
    }

    public ICommandResult Handle(TCommand command)
    {
        try
        {
            var failures = _validators.Select(x => x.Validate(command)).Where(x => !x.IsValid).ToList();

            if (failures.Any())
            {
                List<KeyValuePair<string, string>> errors = failures.SelectMany(x => x.Errors)
                    .Select(x => new KeyValuePair<string, string>(x.PropertyName, x.ErrorMessage)).ToList();

                _logger.LogWarning(LoggingUtils.ValidationFailed, "Validation failed for {Command}: {ValidationErrors}",
                    command.GetTypeFullName(), string.Join(Environment.NewLine, errors));

                return CommandResult.Fail(new ValidationError(errors));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(LoggingUtils.UnexpectedException, ex,
                "Unexpected exception occurred when validating command: {Command}", command.GetTypeFullName());

            return CommandResult.Fail(new UnexpectedError(
                $"Unexpected exception occurred while validating command: {command.GetTypeFullName()}", ex));
        }

        _logger.LogDebug(LoggingUtils.ValidationSuccess, "Validation passed for {Command}", command.GetTypeFullName());

        return _decorated.Handle(command);
    }
}