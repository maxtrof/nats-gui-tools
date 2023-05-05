namespace Domain.Models;

/// <summary>
/// App settings
/// </summary>
public sealed class AppSettings
{
    public List<NatsServerSettings> Servers { get; set; } = default!;

    public static AppSettings InitDefaults()
    {
        return new AppSettings
        {
            Servers = new List<NatsServerSettings>()
        };
    }
}