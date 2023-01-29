using System.Diagnostics;
using EssentialFrame.Cqrs.Events.Core.Interfaces;
using EssentialFrame.Extensions;
using EssentialFrame.Serialization.Interfaces;
using Microsoft.Extensions.Logging;

namespace EssentialFrame.Cqrs.Events.Logging.Decorators;

public sealed class LoggingAsyncEventsHandlerDecorator<TEvent> : IAsyncEventHandler<TEvent> where TEvent : class, IEvent
{
    private readonly IAsyncEventHandler<TEvent> _decorated;
    private readonly ILogger<LoggingAsyncEventsHandlerDecorator<TEvent>> _logger;
    private readonly ISerializer _serializer;

    internal LoggingAsyncEventsHandlerDecorator(ILogger<LoggingAsyncEventsHandlerDecorator<TEvent>> logger,
        IAsyncEventHandler<TEvent> decorated, ISerializer serializer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public async Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        EventId executing = LoggingUtils.EventExecuting;
        EventId executed = LoggingUtils.EventExecuted;
        string eventName = @event.GetTypeFullName();

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["EventId"] = @event.EventIdentifier,
                   ["AggregateId"] = @event.AggregateIdentifier,
                   ["ExpectedVersion"] = @event.ExpectedVersion ?? -1,
                   ["UserId"] = @event.UserIdentity,
                   ["ServiceId"] = @event.ServiceIdentity,
                   ["TenantId"] = @event.TenantIdentity
               }))
        {
            string serializedEvent = _serializer.Serialize(@event);

            _logger.LogInformation(executing, "[START] {EventName}. Params: {SerializedEvent}", eventName,
                serializedEvent);

            Stopwatch stopwatch = new();

            try
            {
                stopwatch.Start();
                await _decorated.HandleAsync(@event, cancellationToken);
                stopwatch.Stop();

                _logger.LogInformation(executed, "[END] {EventName}: {ExecutionTime}[ms]", eventName,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(LoggingUtils.UnexpectedException, ex, "[END] {EventName}: {ExecutionTime}[ms]",
                    eventName, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}