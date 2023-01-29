using EssentialFrame.Cqrs.Commands.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Errors;

public class DomainRuleError : ICommandError
{
    public DomainRuleError(string message, string brokenRule, IDictionary<string, string> parameters = null)
    {
        Message = message;
        BrokenRule = brokenRule;
        Parameters = parameters;
    }

    public string Message { get; }

    public string BrokenRule { get; }

    public IDictionary<string, string> Parameters { get; }
}