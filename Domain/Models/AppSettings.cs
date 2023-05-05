namespace Domain.Models;

/// <summary>
/// App settings
/// </summary>
public sealed class AppSettings
{
    /// <summary>
    /// User defined servers
    /// </summary>
    public List<NatsServerSettings> Servers { get; set; } = default!;
    /// <summary>
    /// User's dictionary
    /// </summary>
    public Dictionary<string, string> UserDictionary { get; set; } = default!;

    public static AppSettings InitDefaults()
    {
        return new AppSettings
        {
            Servers = new List<NatsServerSettings>(),
            UserDictionary = new Dictionary<string, string>()
        };
    }
}