using System;
using System.Text.RegularExpressions;

class SimpleHtmlFormatter
{
    private static readonly Dictionary<string, string> colorCodes = new()
    {
        { "black", "\x1b[30m" },
        { "red", "\x1b[91m" },
        { "green", "\x1b[92m" },
        { "yellow", "\x1b[93m" },
        { "blue", "\x1b[94m" },
        { "magenta", "\x1b[95m" },
        { "cyan", "\x1b[96m" },
        { "white", "\x1b[97m" },
        { "gray", "\x1b[90m" },
        { "darkred", "\x1b[31m" },
        { "darkgreen", "\x1b[32m" },
        { "darkyellow", "\x1b[33m" },
        { "darkblue", "\x1b[34m" },
        { "darkmagenta", "\x1b[35m" },
        { "darkcyan", "\x1b[36m" },
        { "darkgray", "\x1b[37m" }
    };

    private const string Reset = "\x1b[0m";
    private const string Bold = "\x1b[1m";
    private const string Italic = "\x1b[3m";
    private const string Underline = "\x1b[4m";
    private const string Strikethrough = "\x1b[9m";

    public static void PrintHtml(string html)
    {
        foreach (var color in colorCodes)
        {
            string pattern = $@"<{color.Key}>(.*?)</{color.Key}>";
            html = Regex.Replace(html, pattern,
                match => $"{colorCodes[color.Key]}{match.Groups[1].Value}{Reset}",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);

            pattern = $@"<color\s*=\s*[""']?{color.Key}[""']?\s*>(.*?)</color>";
            html = Regex.Replace(html, pattern,
                match => $"{colorCodes[color.Key]}{match.Groups[1].Value}{Reset}",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        html = html.Replace("<b>", "\x1b[1m")
                  .Replace("</b>", "\x1b[0m")
                  .Replace("<i>", "\x1b[3m")
                  .Replace("</i>", "\x1b[0m");

        html = Regex.Replace(html, "<.*?>", "");
        Console.WriteLine(html.Trim());
    }
}