using System.Diagnostics;
using EssentialFrame.Cqrs.Commands.Core.Interfaces;
using EssentialFrame.Extensions;
using EssentialFrame.Serialization.Interfaces;
using Microsoft.Extensions.Logging;

namespace EssentialFrame.Cqrs.Commands.Logging.Decorators;

internal sealed class LoggingCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _decorated;
    private readonly ILogger<LoggingCommandHandlerDecorator<TCommand>> _logger;
    private readonly ISerializer _serializer;

    public LoggingCommandHandlerDecorator(ILogger<LoggingCommandHandlerDecorator<TCommand>> logger,
        ICommandHandler<TCommand> decorated, ISerializer serializer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public ICommandResult Handle(TCommand command)
    {
        ICommandResult commandResponse;
        EventId executing = LoggingUtils.CommandExecuting;
        EventId executed = LoggingUtils.CommandExecuted;
        string commandName = command.GetTypeFullName();

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["CommandId"] = command.CommandIdentifier,
                   ["AggregateId"] = command.AggregateIdentifier,
                   ["ExpectedVersion"] = command.ExpectedVersion ?? -1,
                   ["UserId"] = command.IdentityContext.User.Identifier,
                   ["ServiceId"] = command.IdentityContext.Service.Identifier,
                   ["TenantId"] = command.IdentityContext.Tenant.Identifier,
                   ["CorrelationId"] = command.IdentityContext.Correlation.Identifier
               }))
        {
            string serializedCommand = _serializer.Serialize(command);

            _logger.LogInformation(executing, "[START] {CommandName}. Params: {SerializedCommand}", commandName,
                serializedCommand);

            Stopwatch stopwatch = new();

            try
            {
                stopwatch.Start();
                commandResponse = _decorated.Handle(command);
                stopwatch.Stop();

                if (commandResponse.IsSuccess)
                {
                    _logger.LogInformation(executed, "[END] {CommandName}: {ExecutionTime}[ms]", commandName,
                        stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    string serializedCommandResponse = _serializer.Serialize(commandResponse);

                    _logger.LogWarning(executed, "[END] {CommandName}: {ExecutionTime}[ms] {SerializedCommandResponse}",
                        commandName, stopwatch.ElapsedMilliseconds, serializedCommandResponse);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(LoggingUtils.UnexpectedException, ex, "[END] {CommandName}: {ExecutionTime}[ms]",
                    commandName, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }

        return commandResponse;
    }
}