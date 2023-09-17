using System.Reflection;
using EssentialFrame.Domain.Core.ValueObjects;
using EssentialFrame.Domain.Core.ValueObjects.Core;
using EssentialFrame.Domain.EventSourcing.Core.Aggregates;
using EssentialFrame.Domain.Exceptions;

namespace EssentialFrame.Domain.EventSourcing.Core.Factories;

public static class EventSourcingGenericAggregateFactory<T, TAggregateIdentifier>
    where T : class, IEventSourcingAggregateRoot<TAggregateIdentifier> where TAggregateIdentifier : TypedGuidIdentifier
{
    public static T CreateAggregate(TAggregateIdentifier aggregateIdentifier)
    {
        ConstructorInfo[] constructors = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (ConstructorInfo constructor in constructors)
        {
            List<string> parameterNames = constructor.GetParameters().Select(p => p.Name).ToList();

            if (parameterNames.Contains(nameof(aggregateIdentifier)))
            {
                return (T)constructor.Invoke(new object[] { aggregateIdentifier });
            }
        }

        throw new MissingConstructorException(typeof(T));
    }

    public static T CreateAggregate(TAggregateIdentifier aggregateIdentifier, int aggregateVersion)
    {
        ConstructorInfo[] constructors = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (ConstructorInfo constructor in constructors)
        {
            List<string> parameterNames = constructor.GetParameters().Select(p => p.Name).ToList();

            if (parameterNames.Contains(nameof(aggregateIdentifier)) &&
                parameterNames.Contains(nameof(aggregateVersion)))
            {
                return (T)constructor.Invoke(new object[] { aggregateIdentifier, aggregateVersion });
            }
        }

        throw new MissingConstructorException(typeof(T));
    }

    public static T CreateAggregate(TAggregateIdentifier aggregateIdentifier, int aggregateVersion,
        TenantIdentifier tenantIdentifier)
    {
        ConstructorInfo[] constructors = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (ConstructorInfo constructor in constructors)
        {
            List<string> parameterNames = constructor.GetParameters().Select(p => p.Name).ToList();

            if (parameterNames.Contains(nameof(aggregateIdentifier)) &&
                parameterNames.Contains(nameof(aggregateVersion)) && parameterNames.Contains(nameof(tenantIdentifier)))
            {
                return (T)constructor.Invoke(new object[] { aggregateIdentifier, aggregateVersion, tenantIdentifier });
            }
        }

        throw new MissingConstructorException(typeof(T));
    }
}