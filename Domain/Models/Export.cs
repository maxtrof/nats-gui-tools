namespace Domain.Models;

/// <summary>
/// Defines export/import data
/// </summary>
public class Export
{
    /// <summary>
    /// Mocks
    /// </summary>
    public List<MockTemplate> MockTemplates { get; set; } = default!;
    /// <summary>
    /// Requests
    /// </summary>
    public List<RequestTemplate> RequestTemplates { get; set; } = default!;
    /// <summary>
    /// Listeners
    /// </summary>
    public List<Listener> Listeners { get; set; } = default!;
}