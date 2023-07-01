namespace Domain.Models;

/// <summary>
/// App settings
/// </summary>
public sealed class AppSettings
{
    /// <summary>
    /// Max size of <see cref="AutoCompletionDictionary"/>
    /// </summary>
    private const int MaxAutoCompletionDictionarySize = 30;
    /// <summary>
    /// Keeps last <see cref="MaxAutoCompletionDictionarySize"/> topic names to provide auto completion
    /// </summary>
    private List<string> AutoCompletionDictionary { get; set; }
    /// <summary>
    /// User defined servers
    /// </summary>
    public List<NatsServerSettings> Servers { get; set; } = default!;
    /// <summary>
    /// User's dictionary
    /// </summary>
    public Dictionary<string, string> UserDictionary { get; set; } = default!;
    /// <summary>
    /// Should we try format JSON responses from NATS
    /// </summary>
    public bool FormatJson { get; set; }
    /// <summary>
    /// Gets a copy of <see cref="AutoCompletionDictionary"/>
    /// </summary>
    public List<string> GetAutoCompletionDictionary() => AutoCompletionDictionary.ToList();
    /// <summary>
    /// Adds a variant to <see cref="AutoCompletionDictionary"/>
    /// If variant already exists - does nothing
    /// If dictionary reached max size - removes first entry and adds new one
    /// </summary>
    /// <param name="variant">Variant to add</param>
    public void AddVariantToAutoCompletionDictionary(string variant)
    {
        if (string.IsNullOrWhiteSpace(variant) || AutoCompletionDictionary.Contains(variant)) return;
        if (AutoCompletionDictionary.Count > MaxAutoCompletionDictionarySize)
        {
            AutoCompletionDictionary.RemoveAt(0);
        }
        AutoCompletionDictionary.Add(variant);
    }

    private AppSettings()
    {
        AutoCompletionDictionary = new List<string>();
    }
    public AppSettings(List<string>? autoCompletionDictionary)
    {
        AutoCompletionDictionary = autoCompletionDictionary ?? new List<string>();
    }

    public static AppSettings InitDefaults()
    {
        return new AppSettings
        {
            Servers = new List<NatsServerSettings>(),
            UserDictionary = new Dictionary<string, string>()
        };
    }
}