using System.Text.RegularExpressions;

namespace BO3SoundSuite.Services;

public static partial class SzcService
{
    [GeneratedRegex("\\\"Type\\\"\\s*:\\s*\\\"ALIAS\\\"", RegexOptions.IgnoreCase)]
    private static partial Regex AliasTypeRegex();

    [GeneratedRegex("\\\"Name\\\"\\s*:\\s*\\\"([^\\\"]*)\\\"", RegexOptions.IgnoreCase)]
    private static partial Regex NameRegex();

    [GeneratedRegex("\\\"Filename\\\"\\s*:\\s*\\\"([^\\\"]*)\\\"", RegexOptions.IgnoreCase)]
    private static partial Regex FilenameRegex();

    public static string BuildAliasBlock(string name, string filename)
        => "{\r\n    \"Type\" : \"ALIAS\",\r\n    \"Name\" : \"" + Escape(name) + "\",\r\n    \"Filename\" : \"" + Escape(filename) + "\",\r\n    \"Specs\" : [ ]\r\n}";

    public static (string Name, string Filename)? GetFirstAlias(string text)
    {
        var range = FindFirstAliasObject(text);
        if (!range.HasValue) return null;
        var block = text.Substring(range.Value.Start, range.Value.Length);
        var n = NameRegex().Match(block);
        var f = FilenameRegex().Match(block);
        return (n.Success ? Unescape(n.Groups[1].Value) : string.Empty, f.Success ? Unescape(f.Groups[1].Value) : string.Empty);
    }

    public static string ReplaceFirstAlias(string text, string name, string filename)
    {
        var block = BuildAliasBlock(name, filename);
        var range = FindFirstAliasObject(text);
        if (range.HasValue)
            return text[..range.Value.Start] + block + text[(range.Value.Start + range.Value.Length)..];
        return InsertAlias(text, block);
    }

    public static string InsertAlias(string text, string name, string filename) => InsertAlias(text, BuildAliasBlock(name, filename));

    private static (int Start, int Length)? FindFirstAliasObject(string text)
    {
        if (string.IsNullOrEmpty(text)) return null;
        var typeMatch = AliasTypeRegex().Match(text);
        if (!typeMatch.Success) return null;

        var stack = new Stack<int>();
        var quoted = false;
        for (var i = 0; i < typeMatch.Index; i++)
        {
            var c = text[i];
            if (c == '"' && !IsEscaped(text, i))
            {
                quoted = !quoted;
                continue;
            }
            if (quoted) continue;
            if (c == '{') stack.Push(i);
            else if (c == '}' && stack.Count > 0) stack.Pop();
        }
        if (stack.Count == 0) return null;

        var start = stack.Peek();
        var close = FindMatchingDelimiter(text, start, '{', '}');
        if (close < 0) return null;
        return (start, close - start + 1);
    }

    private static string InsertAlias(string text, string block)
    {
        var sourcesIndex = text.IndexOf("\"Sources\"", StringComparison.OrdinalIgnoreCase);
        if (sourcesIndex < 0)
            return "{\r\n  \"Sources\" : [\r\n    " + Indent(block, 4) + "\r\n  ]\r\n}\r\n";

        var open = text.IndexOf('[', sourcesIndex);
        if (open < 0) return text + Environment.NewLine + block;
        var close = FindMatchingDelimiter(text, open, '[', ']');
        if (close < 0) return text + Environment.NewLine + block;

        var body = text[(open + 1)..close].Trim();
        var prefix = body.Length == 0 ? "\r\n    " : body.EndsWith(',') ? "\r\n    " : ",\r\n    ";
        return text[..close] + prefix + Indent(block, 4) + "\r\n  " + text[close..];
    }

    private static int FindMatchingDelimiter(string text, int openIndex, char openChar, char closeChar)
    {
        var depth = 0;
        var quoted = false;
        for (var i = openIndex; i < text.Length; i++)
        {
            var c = text[i];
            if (c == '"' && !IsEscaped(text, i))
            {
                quoted = !quoted;
                continue;
            }
            if (quoted) continue;
            if (c == openChar) depth++;
            else if (c == closeChar && --depth == 0) return i;
        }
        return -1;
    }

    private static bool IsEscaped(string text, int index)
    {
        var backslashes = 0;
        for (var i = index - 1; i >= 0 && text[i] == '\\'; i--) backslashes++;
        return backslashes % 2 != 0;
    }

    private static string Escape(string s) => (s ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");

    private static string Unescape(string s)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        return Regex.Unescape(s);
    }

    private static string Indent(string s, int spaces)
    {
        var pad = new string(' ', spaces);
        return string.Join("\r\n", s.Replace("\r\n", "\n").Split('\n').Select((x, i) => i == 0 ? x : pad + x));
    }
}
