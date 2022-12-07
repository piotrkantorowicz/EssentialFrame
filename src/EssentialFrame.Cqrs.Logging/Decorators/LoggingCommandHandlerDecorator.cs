using System.Diagnostics;
using EssentialFrame.Cqrs.Commands.Core;
using EssentialFrame.Cqrs.Interfaces;
using EssentialFrame.Serialization;
using Microsoft.Extensions.Logging;

namespace EssentialFrame.Cqrs.Logging.Decorators;

public sealed class LoggingCommandDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _decorated;
    private readonly ILogger<LoggingCommandDecorator<TCommand>> _logger;
    private readonly ISerializer _serializer;

    internal LoggingCommandDecorator(ILogger<LoggingCommandDecorator<TCommand>> logger,
                                     ICommandHandler<TCommand> decorated,
                                     ISerializer serializer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public ICommandResult Handle(TCommand command)
    {
        ICommandResult commandResponse;
        var executing = LoggingUtils.CommandExecuting;
        var executed = LoggingUtils.CommandExecuted;
        var commandName = command.GetType().FullName;

        using (_logger.BeginScope(new Dictionary<string, object>
                                  {
                                      ["CommandId"] = command.CommandIdentifier,
                                      ["AggregateId"] = command.AggregateIdentifier,
                                      ["ExpectedVersion"] = command.ExpectedVersion ?? -1,
                                      ["TenantId"] = command.IdentityTenant,
                                      ["UserId"] = command.IdentityUser
                                  }))
        {
            var serializedCommand = _serializer.Serialize(command);

            _logger.LogInformation(executing,
                                   "[START] {CommandName}. Params: {SerializedCommand}",
                                   commandName,
                                   serializedCommand);

            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                commandResponse = _decorated.Handle(command);
                stopwatch.Stop();

                if (commandResponse.Ok)
                {
                    _logger.LogInformation(executed,
                                           "[END] {CommandName}: {ExecutionTime}[ms]",
                                           commandName,
                                           stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    var serializedCommandResponse = _serializer.Serialize(commandResponse);

                    _logger.LogWarning(executed,
                                       "[END] {CommandName}: {ExecutionTime}[ms] {SerializedCommandResponse}",
                                       commandName,
                                       stopwatch.ElapsedMilliseconds,
                                       serializedCommandResponse);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(LoggingUtils.UnexpectedException,
                                 ex,
                                 "[END] {CommandName}: {ExecutionTime}[ms]",
                                 commandName,
                                 stopwatch.ElapsedMilliseconds);

                throw;
            }
        }

        return commandResponse;
    }

    public async Task<ICommandResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        ICommandResult commandResponse;
        var executing = LoggingUtils.CommandExecuting;
        var executed = LoggingUtils.CommandExecuted;
        var commandName = command.GetType().FullName;

        using (_logger.BeginScope(new Dictionary<string, object>
                                  {
                                      ["CommandId"] = command.CommandIdentifier,
                                      ["AggregateId"] = command.AggregateIdentifier,
                                      ["ExpectedVersion"] = command.ExpectedVersion ?? -1,
                                      ["TenantId"] = command.IdentityTenant,
                                      ["UserId"] = command.IdentityUser
                                  }))
        {
            var serializedCommand = _serializer.Serialize(command);

            _logger.LogInformation(executing,
                                   "[START] {CommandName}. Params: {SerializedCommand}",
                                   commandName,
                                   serializedCommand);

            var stopwatch = new Stopwatch();

            try
            {
                stopwatch.Start();
                commandResponse = await _decorated.HandleAsync(command, cancellationToken);
                stopwatch.Stop();

                if (commandResponse.Ok)
                {
                    _logger.LogInformation(executed,
                                           "[END] {CommandName}: {ExecutionTime}[ms]",
                                           commandName,
                                           stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    var serializedCommandResponse = _serializer.Serialize(commandResponse);

                    _logger.LogWarning(executed,
                                       "[END] {CommandName}: {ExecutionTime}[ms] {SerializedCommandResponse}",
                                       commandName,
                                       stopwatch.ElapsedMilliseconds,
                                       serializedCommandResponse);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(LoggingUtils.UnexpectedException,
                                 ex,
                                 "[END] {CommandName}: {ExecutionTime}[ms]",
                                 commandName,
                                 stopwatch.ElapsedMilliseconds);

                throw;
            }
        }

        return commandResponse;
    }
}
