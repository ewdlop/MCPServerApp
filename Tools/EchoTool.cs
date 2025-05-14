using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Linq;

[McpServerToolType]
public static class EchoTool
{
    [McpServerTool, Description("Echoes the message back to the client.")]
    public static string Echo(string message) => $"Hello from C#: {message}";

    [McpServerTool, Description("Echoes in reverse the message sent by the client.")]
    public static string ReverseEcho(string message) => new string(message.Reverse().ToArray());

    [McpServerTool, Description("Echoes in reverse the message sent by the client.")]
    public static string Yell(string message) => message.ToUpper();

    [McpServerTool, Description("Counts the number of words in the message.")]
    public static int WordCount(string message) =>
        string.IsNullOrWhiteSpace(message) ? 0 : message.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

    [McpServerTool, Description("Repeats the message N times, separated by spaces.")]
    public static string Repeat(string message, int times) =>
        times <= 0 ? "" : string.Join(' ', Enumerable.Repeat(message, times));

    [McpServerTool, Description("Checks if the message is a palindrome (ignores case and spaces).")]
    public static bool IsPalindrome(string message)
    {
        var cleaned = new string(message.Where(char.IsLetterOrDigit).Select(char.ToLower).ToArray());
        return cleaned.SequenceEqual(cleaned.Reverse());
    }

    [McpServerTool, Description("Converts the message to Title Case.")]
    public static string TitleCase(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return message;
        return string.Join(' ', message
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
    }

    [McpServerTool, Description("Counts the number of characters in the message (excluding spaces).")]
    public static int CharCount(string message) =>
        string.IsNullOrEmpty(message) ? 0 : message.Count(c => !char.IsWhiteSpace(c));

    [McpServerTool, Description("Extracts all digits from the message as a string.")]
    public static string ExtractDigits(string message) =>
        new string(message.Where(char.IsDigit).ToArray());

    [McpServerTool, Description("Finds the longest word in the message.")]
    public static string LongestWord(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return "";
        return message
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .OrderByDescending(w => w.Length)
            .FirstOrDefault() ?? "";
    }

    [McpServerTool, Description("Counts the number of vowels in the message.")]
    public static int VowelCount(string message)
    {
        if (string.IsNullOrEmpty(message)) return 0;
        var vowels = "aeiouAEIOU";
        return message.Count(c => vowels.Contains(c));
    }

    [McpServerTool, Description("Replaces all occurrences of a substring with another substring.")]
    public static string Replace(string message, string oldValue, string newValue)
    {
        if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(oldValue)) return message;
        return message.Replace(oldValue, newValue);
    }
}
