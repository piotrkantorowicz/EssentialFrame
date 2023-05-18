namespace EssentialFrame.Domain.Rules;

public interface IBusinessRule
{
    string Message { get; }

    IDictionary<string, object> Parameters { get; }

    bool IsBroken();
}