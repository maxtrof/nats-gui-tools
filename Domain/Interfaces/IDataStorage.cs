using Domain.Models;

namespace Domain.Interfaces;

/// <summary>
/// Keeps app data in memory and saves/load it from persistant storage when needed
/// </summary>
public interface IDataStorage
{
    /// <summary>
    /// Initializes storage. Should be called before usage.
    /// </summary>
    Task InitializeAsync();
    /// <summary>
    /// Checks if data has to be saved and saves it. 
    /// </summary>
    Task SaveDataIfNeeded();
    /// <summary>
    /// Application settings
    /// </summary>
    AppSettings AppSettings { get; set; }
    /// <summary>
    /// Request templates
    /// </summary>
    List<RequestTemplate> RequestTemplates { get; set; }
    /// <summary>
    /// Mock templates
    /// </summary>
    List<MockTemplate> MockTemplates { get; set; }
    /// <summary>
    /// Increments App settings version (will trigger save on <see cref="SaveDataIfNeeded"/>)
    /// </summary>
    void IncAppSettingsVersion();
    /// <summary>
    /// Increments Request Templates version (will trigger save on <see cref="SaveDataIfNeeded"/>)
    /// </summary>
    void IncRequestTemplatesVersion();
    /// <summary>
    /// Increments Mock Templates version (will trigger save on <see cref="SaveDataIfNeeded"/>)
    /// </summary>
    void IncMockTemplatesVersion();
}