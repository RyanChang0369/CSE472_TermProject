using System;
using System.Globalization;
using System.Text.RegularExpressions;

/// <summary>
/// Contains methods that extend strings and Regex.
/// 
/// <br/>
/// 
/// Authors: Ryan Chang (2023)
/// </summary>
public static class StringExt
{
    #region Variables
    private static readonly Regex memberPrefixRx = new("^((m?_)|k| +)([A-Z])",
        RegexOptions.Compiled);

    private static readonly Regex capitalLetterRx = new("([A-Z])",
        RegexOptions.Compiled);
    #endregion


    #region Methods
    /// <summary>
    /// Replicates <see
    /// href="https://docs.unity3d.com/ScriptReference/ObjectNames.NicifyVariableName.html"/>.
    /// </summary>
    /// <param name="str">The input string to nicify.</param>
    /// <returns>A nicified string</returns>
    public static string Nicify(this string str)
    {
        // First replace the member, underscore, and k prefixes (and also whitespace).
        str = memberPrefixRx.Replace(str, "$3").Trim();

        // Make title case.
        str = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);

        // Add spaces.
        str = capitalLetterRx.Replace(str, " $1");
        return str.Trim();
    }

    /// <summary>
    /// Splice <paramref name="str"/> via <paramref name="separator"/> into a
    /// array of strings, use <paramref name="range"/> to select which words to
    /// keep, then re-joins the array by <paramref name="separator"/>.
    /// </summary>
    /// <param name="str">The string to operate on.</param>
    /// <param name="range">The range of words to select.</param>
    /// <param name="separator">The separator to split <paramref name="str"/>
    /// on.</param>
    /// <param name="options">String split options.</param>
    public static string SpliceWords(this string str, System.Range range,
        char separator, StringSplitOptions options = StringSplitOptions.None)
    {
        return string.Join(
            separator,
            str.Split(separator, options)[range]
        );
    }

    /// <inheritdoc cref="SpliceWords(string, System.Range, char,
    /// StringSplitOptions)"/>
    public static string SpliceWords(this string str, System.Range range,
        string separator, StringSplitOptions options = StringSplitOptions.None)
    {
        return string.Join(
            separator,
            str.Split(separator, options)[range]
        );
    }
    #endregion
}
