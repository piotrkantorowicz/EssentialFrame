namespace EssentialFrame.Domain.Rules.Base;

public abstract class BaseBusinessRule : IBusinessRule
{
    public abstract string Message { get; }

    public IDictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

    public abstract bool IsBroken();

    public abstract void AddExtraParameters();
}