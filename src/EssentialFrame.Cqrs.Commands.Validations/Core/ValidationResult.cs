namespace EssentialFrame.Cqrs.Commands.Validations.Core;

public sealed class ValidationResult
{
    private List<ValidationProblem> _errors;

    internal ValidationResult() => _errors = new List<ValidationProblem>();

    internal ValidationResult(IEnumerable<ValidationProblem> errors)
    {
        _errors = errors.Where(failure => failure != null).ToList();
    }

    public bool IsValid => Errors.Count == 0;

    public List<ValidationProblem> Errors
    {
        get => _errors;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _errors = value.Where(error => error != null).ToList();
        }
    }

    public override string ToString() => ToString(Environment.NewLine);

    public IDictionary<string, string[]> ToDictionary()
    {
        return Errors
               .GroupBy(x => x.PropertyName)
               .ToDictionary(g => g.Key,
                             g => g.Select(x => x.ErrorMessage).ToArray());
    }

    private string ToString(string separator)
    {
        return string.Join(separator, _errors.Select(failure => failure.ErrorMessage));
    }
}

