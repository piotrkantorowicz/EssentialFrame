using System;
using System.Linq.Expressions;

namespace EssentialFrame.Specifications;

public abstract class Specification<T>
{
    protected abstract Expression<Func<T, bool>> AsPredicateExpression();

    public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
    {
        return specification.AsPredicateExpression();
    }

    public bool Check(T obj)
    {
        return AsPredicateExpression().Compile().Invoke(obj);
    }
}