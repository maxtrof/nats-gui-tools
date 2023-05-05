using Domain.Models;

namespace Domain.Interfaces;

/// <summary>
/// Describes application data repository
/// </summary>
public interface IAppDataRepository
{
    /// <summary>
    /// Loads default Request Templates (so user can have same state on app open)
    /// </summary>
    Task<List<RequestTemplate>> LoadDefaultRequestTemplatesAsync();
    /// <summary>
    /// Loads default Mock Templates (so user can have same state on app open)
    /// </summary>
    Task<List<MockTemplate>> LoadDefaultMockTemplatesAsync();
    /// <summary>
    /// Loads default application settings
    /// </summary>
    Task<AppSettings> LoadAppSettingsAsync();
    
    /// <summary>
    /// Saves default Request Templates (so user can have same state on app open)
    /// </summary>
    Task SaveDefaultRequestTemplatesAsync(List<RequestTemplate> templates);
    /// <summary>
    /// Saves default Mock Templates (so user can have same state on app open)
    /// </summary>
    Task SaveDefaultMockTemplatesAsync(List<MockTemplate> templates);
    /// <summary>
    /// Saves default application settings
    /// </summary>
    Task SaveAppSettingsAsync(AppSettings settings);
}