﻿using System.Reflection;
using EssentialFrame.Domain.Exceptions;

namespace EssentialFrame.Domain.Factories;

public static class GenericAggregateFactory<T>
{
    public static T CreateAggregate(Guid aggregateIdentifier, int aggregateVersion)
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
}