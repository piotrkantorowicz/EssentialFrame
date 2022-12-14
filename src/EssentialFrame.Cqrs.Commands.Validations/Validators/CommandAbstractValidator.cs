using System.Linq.Expressions;
using EssentialFrame.Cqrs.Commands.Interfaces;
using EssentialFrame.Cqrs.Commands.Validations.Core;
using EssentialFrame.Cqrs.Commands.Validations.Core.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Validations.Validators;

public abstract class CommandAbstractValidator<TCommand> : ICommandValidator<TCommand>
    where TCommand : class, ICommand
{
    private readonly IList<ICommandPropertyRule<TCommand>> _rules = new List<ICommandPropertyRule<TCommand>>();

    public ValidationResult Validate(TCommand command)
    {
        var validationProblems = _rules
                                 .Select(rule => rule.Validate(command))
                                 .Where(validationProblem => validationProblem != null)
                                 .ToList();

        return new ValidationResult(validationProblems);
    }

    protected void RuleFor<TProperty>(Expression<Func<TCommand, TProperty>> expression,
                                      Expression<Func<TCommand, TProperty, IValidationRule>> ruleExpression)
    {
        var rule = CommandPropertyRule<TCommand, TProperty>.Create(expression, ruleExpression);

        _rules.Add(rule);
    }
}
