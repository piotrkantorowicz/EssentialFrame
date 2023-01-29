using System.Collections.ObjectModel;
using EssentialFrame.Cqrs.Commands.Errors.Interfaces;

namespace EssentialFrame.Cqrs.Commands.Errors;

public class ValidationError : ICommandError
{
    private readonly List<KeyValuePair<string, string>> _errors;

    public ValidationError()
    {
        _errors = new List<KeyValuePair<string, string>>();
        ValidationErrors = new ReadOnlyCollection<KeyValuePair<string, string>>(_errors);
    }

    public ValidationError(string property, string message) : this()
    {
        AddError(property, message);
    }

    public ValidationError(IEnumerable<KeyValuePair<string, string>> errors) : this()
    {
        if (errors is null)
        {
            throw new ArgumentNullException(nameof(errors));
        }

        foreach (KeyValuePair<string, string> error in errors)
        {
            AddError(error.Key, error.Value);
        }
    }

    public ReadOnlyCollection<KeyValuePair<string, string>> ValidationErrors { get; }

    public string Message =>
        $"Validation failed. {string.Join(". ", ValidationErrors.Select(x => $"{x.Key} = {x.Value}"))}";

    public ValidationError AddError(string property, string message)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        _errors.Add(new KeyValuePair<string, string>(property, message));
        return this;
    }
}