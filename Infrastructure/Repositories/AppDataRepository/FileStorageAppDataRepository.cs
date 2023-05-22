using System.Text.Json;
using Application.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure.Repositories.AppDataRepository.Dto;

namespace Infrastructure.Repositories.AppDataRepository;

/// <summary>
/// File system implementation of <see cref="IAppDataRepository"/>
/// </summary>
public sealed class FileStorageAppDataRepository : IAppDataRepository
{
    private const string SettingsFileName = "settings.json";
    private const string MocksFileName = "mocks.json";
    private const string RequestsFileName = "requests.json";
    
    /// <summary>
    /// Folder for saving local files
    /// </summary>
    private readonly string _localDataFolder;

    /// <summary>
    /// Ensures we'll read or write only one file at a time
    /// </summary>
    private static readonly SemaphoreSlim FileAccessSemaphore = new (1);
    
    public FileStorageAppDataRepository()
    {
        _localDataFolder = LocateAndCreateIfNeededLocalDataFolder();
    }
    
    /// <inheritdoc />
    public async Task<List<RequestTemplate>> LoadDefaultRequestTemplatesAsync()
    {
        var requests = await LoadJsonFile<RequestTemplatesDto>(RequestsFileName);
        if (requests is null) return new List<RequestTemplate>();
        return requests.ToDomain();
    }

    /// <inheritdoc />
    public async Task<List<MockTemplate>> LoadDefaultMockTemplatesAsync()
    {
        var mocks = await LoadJsonFile<MockTemplatesDto>(MocksFileName);
        if (mocks is null) return new List<MockTemplate>();
        return mocks.ToDomain();
    }

    /// <inheritdoc />
    public async Task<AppSettings> LoadAppSettingsAsync()
    {
        var settings = await LoadJsonFile<AppSettingsDto>(SettingsFileName);
        if (settings is null) return AppSettings.InitDefaults();
        return settings.ToDomain();
    }

    /// <inheritdoc />
    public Task SaveDefaultRequestTemplatesAsync(List<RequestTemplate> templates)
    {
        return WriteJsonFile(RequestsFileName, templates.ToDto());
    }

    /// <inheritdoc />
    public Task SaveDefaultMockTemplatesAsync(List<MockTemplate> templates)
    {
        return WriteJsonFile(MocksFileName, templates.ToDto());
    }

    /// <inheritdoc />
    public Task SaveAppSettingsAsync(AppSettings settings)
    {
        return WriteJsonFile(SettingsFileName, settings.ToDto());
    }

    /// <inheritdoc />
    public Task Export(string fileName, Export export)
    {
        var exportData = new ExportDto(export.MockTemplates.ToDto(), export.RequestTemplates.ToDto());
        return WriteJsonFile(fileName, exportData);
    }

    /// <inheritdoc />
    public async Task<Export> Import(string fileName)
    {
        var data = await LoadJsonFile<ExportDto>(fileName);
        if (data is null) throw new ImportException("Failed to load file");
        return new Export
        {
            MockTemplates = data.MockTemplates.ToDomain(),
            RequestTemplates = data.RequestTemplates.ToDomain()
        };
    }

    /// <summary>
    /// Loads json file and deserializes it
    /// </summary>
    /// <param name="fileName">File name</param>
    private async Task<T?> LoadJsonFile<T>(string fileName)
    {
        var path = Path.Combine(_localDataFolder, fileName);
        if (!File.Exists(path)) return default;
        
        await FileAccessSemaphore.WaitAsync();
        try
        {
            await using var stream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
        finally
        {
            FileAccessSemaphore.Release();
        }
    }

    /// <summary>
    /// Serializes object and writes it to file
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="data">Object</param>
    private async Task WriteJsonFile(string fileName, object data)
    {
        await FileAccessSemaphore.WaitAsync();
        try
        {
            var content = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(Path.Combine(_localDataFolder, fileName), content);
        }
        finally
        {
            FileAccessSemaphore.Release();
        }
    }

    /// <summary>
    /// Locates folder to save files (and creates it if needed) (cross-platform)
    /// </summary>
    /// <returns>Folder full path</returns>
    private string LocateAndCreateIfNeededLocalDataFolder()
    {
        var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify), "nats-gui-tools");
        Directory.CreateDirectory(appData);
        return appData;
    }
}