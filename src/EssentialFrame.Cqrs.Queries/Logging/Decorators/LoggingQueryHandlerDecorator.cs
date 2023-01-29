using System.Diagnostics;
using EssentialFrame.Cqrs.Queries.Core.Interfaces;
using EssentialFrame.Extensions;
using EssentialFrame.Serialization.Interfaces;
using Microsoft.Extensions.Logging;

namespace EssentialFrame.Cqrs.Queries.Logging.Decorators;

public sealed class LoggingQueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : class, IQuery<TResult> where TResult : IQueryResult<TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _decorated;
    private readonly ILogger<LoggingQueryHandlerDecorator<TQuery, TResult>> _logger;
    private readonly ISerializer _serializer;

    internal LoggingQueryHandlerDecorator(ILogger<LoggingQueryHandlerDecorator<TQuery, TResult>> logger,
        IQueryHandler<TQuery, TResult> decorated, ISerializer serializer)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _decorated = decorated ?? throw new ArgumentNullException(nameof(decorated));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public TResult Handle(TQuery query)
    {
        TResult queryResponse;
        EventId executing = LoggingUtils.QueryExecuting;
        EventId executed = LoggingUtils.QueryExecuted;
        string queryName = query.GetTypeFullName();

        using (_logger.BeginScope(new Dictionary<string, object>
               {
                   ["QueryId"] = query.QueryIdentifier,
                   ["UserId"] = query.UserIdentity,
                   ["ServiceId"] = query.ServiceIdentity,
                   ["TenantId"] = query.TenantIdentity
               }))
        {
            string serializedQuery = _serializer.Serialize(query);

            _logger.LogInformation(executing, "[START] {QueryName}. Params: {SerializedQuery}", queryName,
                serializedQuery);

            Stopwatch stopwatch = new();

            try
            {
                stopwatch.Start();
                queryResponse = _decorated.Handle(query);
                stopwatch.Stop();

                if (queryResponse.Ok)
                {
                    _logger.LogInformation(executed, "[END] {QueryName}: {ExecutionTime}[ms]", queryName,
                        stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    string serializedQueryResponse = _serializer.Serialize(queryResponse);

                    _logger.LogWarning(executed, "[END] {QueryName}: {ExecutionTime}[ms] {SerializedQueryResponse}",
                        queryName, stopwatch.ElapsedMilliseconds, serializedQueryResponse);
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(LoggingUtils.UnexpectedException, ex, "[END] {QueryName}: {ExecutionTime}[ms]",
                    queryName, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }

        return queryResponse;
    }
}