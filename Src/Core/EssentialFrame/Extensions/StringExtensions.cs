using System.Text.RegularExpressions;

namespace EssentialFrame.Extensions;

public static partial class StringExtensions
{
    public static bool IsAlphaNumeric(this string value)
    {
        return AlphaNumericRegex().IsMatch(value);
    }

    public static bool IsAlphaNumericWithSpaces(this string value)
    {
        return AlphaNumericWithSpacesRegex().IsMatch(value);
    }

    public static bool IsAlphaNumericWithCommonCharacters(this string value)
    {
        return AlphaNumericWithCommonCharactersRegex().IsMatch(value);
    }

    public static bool IsAlphaNumericWithSpacesAndCommonCharacters(this string value)
    {
        return AlphaNumericWithSpacesAndCommonCharactersRegex().IsMatch(value);
    }

    [GeneratedRegex(@"^[a-zA-Z0-9]+$", RegexOptions.Multiline)]
    private static partial Regex AlphaNumericRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9 ]+$", RegexOptions.Multiline)]
    private static partial Regex AlphaNumericWithSpacesRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9.,!?'-\\/]*$", RegexOptions.Multiline)]
    private static partial Regex AlphaNumericWithCommonCharactersRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9 .,!?'-\\/]*$", RegexOptions.Multiline)]
    private static partial Regex AlphaNumericWithSpacesAndCommonCharactersRegex();
}