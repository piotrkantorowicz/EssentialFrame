using System.Linq.Expressions;
using System.Reflection;
using EssentialFrame.Cqrs.Commands.Validations.Core.Interfaces;
using EssentialFrame.Cqrs.Commands.Validations.Extensions;

namespace EssentialFrame.Cqrs.Commands.Validations.Core;

internal sealed class CommandPropertyRule<TCommand, TProperty> : ICommandPropertyRule<TCommand>
{
    private CommandPropertyRule(MemberInfo member,
                                Func<TCommand, TProperty, IValidationRule> ruleExpression)
    {
        Member = member;
        RuleExpression = ruleExpression;
    }

    public MemberInfo Member { get; }

    public Func<TCommand, TProperty, IValidationRule> RuleExpression { get; }

    public ValidationProblem Validate(TCommand commandContext)
    {
        var property = commandContext
                       .GetType()
                       .GetProperty(Member.Name)
                       ?.GetValue(commandContext, null);

        if (property is not TProperty commandProperty)
        {
            throw new ArgumentException("Type provided when building a rule is incompatible with command.",
                                        Member.Name);
        }

        var validationRule = RuleExpression(commandContext, commandProperty);

        return !validationRule.IsValid ? new ValidationProblem(validationRule) : null;
    }

    public static ICommandPropertyRule<TCommand> Create(Expression<Func<TCommand, TProperty>> propertyExpression,
                                                        Expression<Func<TCommand, TProperty, IValidationRule>>
                                                            ruleExpression)
    {
        var member = propertyExpression.GetMember();
        var expression = ruleExpression.Compile();

        return new CommandPropertyRule<TCommand, TProperty>(member,
                                                            (command, prop) => expression(command, prop));
    }
}

