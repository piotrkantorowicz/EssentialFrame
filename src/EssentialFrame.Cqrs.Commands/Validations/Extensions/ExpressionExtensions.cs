using System.Linq.Expressions;
using System.Reflection;

namespace EssentialFrame.Cqrs.Commands.Validations.Extensions;

internal static class ExpressionExtensions
{
    internal static MemberInfo GetMember<T, TProperty>(this Expression<Func<T, TProperty>> expression)
    {
        if (RemoveUnary(expression.Body) is not MemberExpression memberExp)
        {
            return null;
        }

        var currentExpr = memberExp.Expression;

        while (true)
        {
            currentExpr = RemoveUnary(currentExpr);

            if (currentExpr is { NodeType: ExpressionType.MemberAccess })
            {
                currentExpr = ((MemberExpression)currentExpr).Expression;
            }
            else
            {
                break;
            }
        }

        if (currentExpr is not { NodeType: ExpressionType.Parameter })
        {
            return null;
        }

        return memberExp.Member;
    }

    private static Expression RemoveUnary(Expression toUnwrap)
    {
        if (toUnwrap is UnaryExpression expression)
        {
            return expression.Operand;
        }

        return toUnwrap;
    }
}





