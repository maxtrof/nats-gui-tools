using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace UI.Helpers;

/// <summary>
/// Helps with JSON formatting
/// </summary>
internal static class JsonFormatter
{
    private static readonly JsonDocumentOptions DocumentOptions = new ()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
    };

    /// <summary>
    /// Check if string is JSON and formats if
    /// </summary>
    /// <param name="input">Source string</param>
    /// <returns>Formatted string if source was json and source string if not.</returns>
    public static string TryFormatJson(string input)
    {
        try
        {
            var parsed = JsonDocument.Parse(input, DocumentOptions);
            return JsonSerializer.Serialize(parsed, SerializerOptions);
        }
        catch
        {
            return input;
        }
    }
}