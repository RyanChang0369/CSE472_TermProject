using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Contains methods that extend string builder.
/// </summary>
public static class StringBuilderExt
{
    #region Append Functions
    /// <summary>
    /// Appends a collection of lines to <paramref name="sb"/>.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="lines">Lines of append.</param>
    public static void AppendLines(this StringBuilder sb,
        IEnumerable<string> lines)
    {
        foreach (var line in lines)
        {
            sb.AppendLine(line);
        }
    }

    /// <summary>
    /// Appends a paragraph to the string builder. Automatically breaks the line
    /// if it goes over lineLen.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="line">The line of text to write.</param>
    /// <param name="lineLen">The maximum length of the line before it is
    /// broken, in characters.</param>
    /// <param name="prefix">Optional prefix to add to the start of line and any
    /// new lines created from breaking up the line.</param>
    /// <param name="suffix">Optional prefix to add to the end of line and any
    /// new lines created from breaking up the line.</param>
    public static void AppendP(this StringBuilder sb,
        string line, int lineLen = 80, string prefix = "", string suffix = "")
    {
        List<string> words = line.Split(" ").ToList();
        int lineBodyLen = lineLen - prefix.Length - suffix.Length;

        if (lineBodyLen < 1)
            throw new System.ArgumentException("Cannot construct a line " +
                "with length (suffix + prefix) " +
                $"{suffix.Length + prefix.Length} and total line length of " +
                $"{lineLen}.");

        int wordsWritten = 0;
        int c;

        while (wordsWritten < words.Count)
        {
            // Write prefix.
            sb.Append(prefix);

            // Write characters until we are out of space for this line.

            if (words[wordsWritten].Length > lineBodyLen)
            {
                // This word is too long. Split it.
                string splitWordL = words[wordsWritten][0..lineBodyLen];
                string splitWordR = words[wordsWritten][((byte)lineBodyLen)..];

                words[wordsWritten] = splitWordL;
                words.Insert(wordsWritten, splitWordR);
            }

            c = 0;
            int nextC = words[wordsWritten].Length;

            // Now write the word.
            while (c + nextC <= lineBodyLen)
            {
                sb.Append(words[wordsWritten]);
                c += words[wordsWritten].Length;
                wordsWritten++;

                if (wordsWritten < words.Count())
                {
                    // There is another word.
                    nextC = words[wordsWritten].Length;

                    if (c < lineBodyLen)
                    {
                        // Add the space.
                        sb.Append(" ");
                        c++;
                    }
                }
                else
                {
                    // Wrote all the words.
                    break;
                }
            }

            // Write suffix (pad accordingly).
            sb.AppendLine(suffix.PadLeft(lineLen - prefix.Length - c));
        }
    }

    /// <summary>
    /// Adds a horizontal line to the string builder.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="lineLen">The length of the line.</param>
    /// <param name="lineChar">The char that will make up the dashed line</param>
    public static void AppendHorzLine(this StringBuilder sb, int lineLen = 80,
        char lineChar = '-')
    {
        sb.AppendLine("".PadLeft(lineLen, lineChar));
    }

    /// <summary>
    /// Adds a section break to the string builder.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="lineLen">The length of the line.</param>
    public static void AppendSectionBreak(this StringBuilder sb,
        int lineLen = 80)
    {
        sb.AppendLine();
        sb.AppendHorzLine(lineLen);
        sb.AppendLine();
    }

    /// <summary>
    /// Adds a formatted title to the string builder.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="title">The text of the title.</param>
    /// <param name="lineLen">The length of the line.</param>
    public static void AppendTitle(this StringBuilder sb, string title,
        int lineLen = 80)
    {
        if (sb.Length > 0)
            sb.AppendLine("\r\n");

        sb.AppendHorzLine(lineLen, '=');
        sb.AppendP(title.ToUpper(), lineLen, "||    ", "    ||");
        sb.AppendHorzLine(lineLen, '=');
        sb.AppendLine();
    }

    /// <summary>
    /// Adds a formatted first level header to the string builder.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="header">The text of the header.</param>
    /// <param name="lineLen">The length of the line.</param>
    public static void AppendH1(this StringBuilder sb, string header,
        int lineLen = 80)
    {
        if (sb.Length > 0)
            sb.AppendLine("\r\n");

        sb.AppendHorzLine(lineLen);
        sb.AppendP(header, lineLen, "| ", " |");
        sb.AppendHorzLine(lineLen);
        sb.AppendLine();
    }

    /// <summary>
    /// Adds a formatted second level header to the string builder.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="header">The text of the header.</param>
    /// <param name="lineLen">The length of the line.</param>
    public static void AppendH2(this StringBuilder sb, string header,
        int lineLen = 80)
    {
        if (sb.Length > 0)
            sb.AppendLine("\r\n");

        sb.AppendP(header, lineLen);
        sb.AppendHorzLine(lineLen);
        sb.AppendLine();
    }

    /// <summary>
    /// Adds a formatted third level header to the string builder.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="header">The text of the header.</param>
    /// <param name="lineLen">The length of the line.</param>
    public static void AppendH3(this StringBuilder sb, string header,
        int lineLen = 80)
    {
        if (sb.Length > 0)
            sb.AppendLine("\r\n");

        sb.AppendP(header, lineLen);
        sb.AppendHorzLine(lineLen / 2, '~');
        sb.AppendLine();
    }

    /// <summary>
    /// Adds a formatted fourth level header to the string builder.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    /// <param name="header">The text of the header.</param>
    /// <param name="lineLen">The length of the line.</param>
    public static void AppendH4(this StringBuilder sb, string header,
        int lineLen = 80)
    {
        if (sb.Length > 0)
            sb.AppendLine("\r\n");

        sb.AppendP(header, lineLen);
        sb.AppendHorzLine(lineLen / 2, '.');
        sb.AppendLine();
    }
    #endregion

    #region Debugging
    /// <summary>
    /// Prints the string builder to the Unity Console using Log.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    public static void Log(this StringBuilder sb)
    {
        UnityEngine.Debug.Log(sb.ToString());
    }

    /// <summary>
    /// Prints the string builder to the Unity Console using LogWarning.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    public static void LogWarning(this StringBuilder sb)
    {
        UnityEngine.Debug.LogWarning(sb.ToString());
    }

    /// <summary>
    /// Prints the string builder to the Unity Console using LogError.
    /// </summary>
    /// <param name="sb">The string builder.</param>
    public static void LogError(this StringBuilder sb)
    {
        UnityEngine.Debug.LogError(sb.ToString());
    }

    #endregion
}