using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Errors;
using EssentialFrame.Cqrs.Commands.Validations.Logging;
using EssentialFrame.Extensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace EssentialFrame.Cqrs.Commands.Validations.Decorators;

public class ValidationAsyncCommandHandlerDecorator<TCommand> : IAsyncCommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly IAsyncCommandHandler<TCommand> _decorated;
    private readonly ILogger<ValidationAsyncCommandHandlerDecorator<TCommand>> _logger;
    private readonly IList<IValidator<TCommand>> _validators;

    internal ValidationAsyncCommandHandlerDecorator(IList<IValidator<TCommand>> validators,
        IAsyncCommandHandler<TCommand> decorated, ILogger<ValidationAsyncCommandHandlerDecorator<TCommand>> logger)
    {
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _validators = validators;
    }

    public async Task<ICommandResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            List<ValidationResult> failures =
                _validators.Select(x => x.Validate(command)).Where(x => !x.IsValid).ToList();

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

        return await _decorated.HandleAsync(command, cancellationToken);
    }
}