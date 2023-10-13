using System;
using System.Linq.Expressions;

namespace EssentialFrame.Specifications;

public class OrSpecification<T> : Specification<T>
{
    private readonly Specification<T>[] _specifications;

    public OrSpecification(params Specification<T>[] specifications)
    {
        _specifications = specifications ?? throw new ArgumentNullException(nameof(specifications));
    }

    protected override Expression<Func<T, bool>> AsPredicateExpression()
    {
        Expression<Func<T, bool>> resultingExpression = null;

        foreach (Specification<T> specification in _specifications)
        {
            if (resultingExpression is null)
            {
                resultingExpression = specification;
                continue;
            }

            resultingExpression = Combine(resultingExpression, specification);
        }

        return resultingExpression;
    }

    private static Expression<Func<T, bool>> Combine(Expression<Func<T, bool>> leftExpression,
        Expression<Func<T, bool>> rightExpression)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T));

        ReplaceExpressionVisitor leftVisitor = new ReplaceExpressionVisitor(leftExpression.Parameters[0], parameter);
        Expression left = leftVisitor.Visit(leftExpression.Body);

        ReplaceExpressionVisitor rightVisitor = new ReplaceExpressionVisitor(rightExpression.Parameters[0], parameter);
        Expression right = rightVisitor.Visit(rightExpression.Body);

        if (left is null || right is null)
        {
            throw new InvalidOperationException("Left or right expression cannot be null.");
        }

        return Expression.Lambda<Func<T, bool>>(Expression.Or(left, right), parameter);
    }
}