using System.Text;

namespace BO3SoundSuite.Services;

public sealed class CsvDocument
{
    public List<string> Headers { get; } = new();
    public List<string[]> Rows { get; } = new();
    public List<string> Comments { get; } = new();
}

public static class CsvService
{
    public static CsvDocument Read(string path)
    {
        var doc = new CsvDocument();
        var lines = File.ReadAllLines(path, DetectEncoding(path));
        var headerFound = false;
        foreach (var raw in lines)
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            if (!headerFound && raw.TrimStart().StartsWith("#"))
            {
                doc.Comments.Add(raw);
                continue;
            }
            if (!headerFound)
            {
                doc.Headers.AddRange(ParseLine(raw));
                headerFound = true;
                continue;
            }
            if (raw.TrimStart().StartsWith("#"))
            {
                doc.Comments.Add(raw);
                continue;
            }
            var cells = ParseLine(raw).ToArray();
            Array.Resize(ref cells, doc.Headers.Count);
            doc.Rows.Add(cells);
        }
        return doc;
    }

    public static void Write(string path, IEnumerable<string> headers, IEnumerable<string[]> rows, IEnumerable<string>? comments = null)
    {
        using var sw = new StreamWriter(path, false, new UTF8Encoding(false));
        sw.WriteLine(string.Join(',', headers.Select(Escape)));
        if (comments != null)
            foreach (var comment in comments) sw.WriteLine(comment);
        foreach (var row in rows)
            sw.WriteLine(string.Join(',', row.Select(Escape)));
    }

    public static List<string> ParseLine(string line)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        var quoted = false;
        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];
            if (ch == '"')
            {
                if (quoted && i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; }
                else quoted = !quoted;
            }
            else if (ch == ',' && !quoted)
            {
                result.Add(sb.ToString()); sb.Clear();
            }
            else sb.Append(ch);
        }
        result.Add(sb.ToString());
        return result;
    }

    private static string Escape(string? value)
    {
        value ??= string.Empty;
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        return value;
    }

    private static Encoding DetectEncoding(string path)
    {
        using var fs = File.OpenRead(path);
        if (fs.Length >= 3)
        {
            var b = new byte[3]; fs.ReadExactly(b);
            if (b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF) return new UTF8Encoding(true);
        }
        return new UTF8Encoding(false, false);
    }
}
