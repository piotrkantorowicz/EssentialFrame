using System.Reflection;
using EssentialFrame.Domain.Events.Core.Aggregates;
using EssentialFrame.Domain.Exceptions;
using EssentialFrame.Identity;

namespace EssentialFrame.Domain.Events.Core.Factories;

public static class GenericAggregateFactory<T> where T : AggregateRoot
{
    public static T CreateAggregate(Guid aggregateIdentifier, IIdentityContext identityContext)
    {
        ConstructorInfo[] constructors = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (ConstructorInfo constructor in constructors)
        {
            List<string> parameterNames = constructor.GetParameters().Select(p => p.Name).ToList();

            if (parameterNames.Contains(nameof(aggregateIdentifier)) &&
                parameterNames.Contains(nameof(identityContext)))
            {
                return (T)constructor.Invoke(new object[] { aggregateIdentifier, identityContext });
            }
        }

        throw new MissingConstructorException(typeof(T));
    }

    public static T CreateAggregate(int aggregateVersion, IIdentityContext identityContext)
    {
        ConstructorInfo[] constructors = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (ConstructorInfo constructor in constructors)
        {
            List<string> parameterNames = constructor.GetParameters().Select(p => p.Name).ToList();

            if (parameterNames.Contains(nameof(aggregateVersion)) && parameterNames.Contains(nameof(identityContext)))
            {
                return (T)constructor.Invoke(new object[] { aggregateVersion, identityContext });
            }
        }

        throw new MissingConstructorException(typeof(T));
    }

    public static T CreateAggregate(Guid aggregateIdentifier, int aggregateVersion, IIdentityContext identityContext)
    {
        ConstructorInfo[] constructors = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (ConstructorInfo constructor in constructors)
        {
            List<string> parameterNames = constructor.GetParameters().Select(p => p.Name).ToList();

            if (parameterNames.Contains(nameof(aggregateIdentifier)) &&
                parameterNames.Contains(nameof(aggregateVersion)) && parameterNames.Contains(nameof(identityContext)))
            {
                return (T)constructor.Invoke(new object[] { aggregateIdentifier, aggregateVersion, identityContext });
            }
        }

        throw new MissingConstructorException(typeof(T));
    }
}