using EssentialFrame.Cqrs.Validations.Core;

namespace EssentialFrame.Cqrs.Validations.CommonRules;

public sealed class GreaterThan : AbstractValidationRule
{
    private readonly int _maxValue;
    private readonly int _value;

    public GreaterThan(string propertyName,
                       int? value,
                       int maxValue)
        : base(propertyName)
    {
        _value = value.GetValueOrDefault();
        _maxValue = maxValue;

        PropertyName = propertyName;
    }

    public override bool IsValid => _value > _maxValue;

    public override string ErrorMessage => $"{PropertyName} have to be greater than {_maxValue}.";
}

public sealed class GreaterOrEqualThan : AbstractValidationRule
{
    private readonly int _maxValue;
    private readonly int _value;

    public GreaterOrEqualThan(string propertyName,
                              int? value,
                              int maxValue)
        : base(propertyName)
    {
        _value = value.GetValueOrDefault();
        _maxValue = maxValue;

        PropertyName = propertyName;
    }

    public override bool IsValid => _value >= _maxValue;

    public override string ErrorMessage => $"{PropertyName} have to be greater than or equal to {_maxValue}.";
}
