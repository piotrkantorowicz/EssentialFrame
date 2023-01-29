using System.Linq.Expressions;
using EssentialFrame.Domain.Exceptions;

namespace EssentialFrame.Domain.Aggregates;

public static class AggregateFactory<T>
{
    private static readonly Func<T> Constructor = CreateTypeConstructor();

    public static T CreateAggregate()
    {
        if (Constructor == null)
        {
            throw new MissingDefaultConstructorException(typeof(T));
        }

        return Constructor();
    }

    private static Func<T> CreateTypeConstructor()
    {
        try
        {
            NewExpression expr = Expression.New(typeof(T));

            Expression<Func<T>> func = Expression.Lambda<Func<T>>(expr);

            return func.Compile();
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}