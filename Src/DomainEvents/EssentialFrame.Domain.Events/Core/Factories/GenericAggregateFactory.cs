using System.Reflection;
using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.Exceptions;

namespace EssentialFrame.Domain.Events.Core.Factories;

public static class GenericAggregateFactory<T> where T : AggregateRoot
{
    public static T CreateAggregate(Guid aggregateIdentifier)
    {
        ConstructorInfo[] constructors = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (ConstructorInfo constructor in constructors)
        {
            List<string> parameterNames = constructor.GetParameters().Select(p => p.Name).ToList();

            if (parameterNames.Contains(nameof(aggregateIdentifier)))
            {
                return (T)constructor.Invoke(new object[] { aggregateIdentifier, 0, default });
            }
        }

        throw new MissingConstructorException(typeof(T));
    }

    public static T CreateAggregate(Guid aggregateIdentifier, int aggregateVersion)
    {
        ConstructorInfo[] constructors = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (ConstructorInfo constructor in constructors)
        {
            List<string> parameterNames = constructor.GetParameters().Select(p => p.Name).ToList();

            if (parameterNames.Contains(nameof(aggregateIdentifier)) &&
                parameterNames.Contains(nameof(aggregateVersion)))
            {
                return (T)constructor.Invoke(new object[] { aggregateIdentifier, aggregateVersion, default });
            }
        }

        throw new MissingConstructorException(typeof(T));
    }

    public static T CreateAggregate(Guid aggregateIdentifier, int aggregateVersion, Guid? tenantIdentifier)
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