using Application.Exceptions;
using Domain.Interfaces;
using Domain.Models;

namespace Application;

/// <summary>
/// App data storage
/// </summary>
public sealed class DataStorage : IDataStorage
{
    private readonly IAppDataRepository _appDataRepository;
    
    private AppSettings? _appSettings;
    private int _appSettingsVersion = 0;
    private int _appSettingsSavedVersion = 0;
    
    private List<RequestTemplate>? _requestTemplates;
    private int _requestTemplatesVersion = 0;
    private int _requestTemplatesSavedVersion = 0;
    
    private List<MockTemplate>? _mockTemplates;
    private int _mockTemplatesVersion = 0;
    private int _mockTemplatesSavedVersion = 0;

    public DataStorage(IAppDataRepository appDataRepository)
    {
        _appDataRepository = appDataRepository ?? throw new ArgumentNullException(nameof(appDataRepository));
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        _appSettings = await _appDataRepository.LoadAppSettingsAsync();
        _requestTemplates = await _appDataRepository.LoadDefaultRequestTemplatesAsync();
        _mockTemplates = await _appDataRepository.LoadDefaultMockTemplatesAsync();
    }

    /// <inheritdoc />
    public async Task SaveDataIfNeeded()
    {
        if (_appSettingsVersion > _appSettingsSavedVersion)
        {
            await _appDataRepository.SaveAppSettingsAsync(AppSettings);
            _appSettingsSavedVersion = _appSettingsVersion;
        }

        if (_requestTemplatesVersion > _requestTemplatesSavedVersion)
        {
            await _appDataRepository.SaveDefaultRequestTemplatesAsync(RequestTemplates);
            _requestTemplatesSavedVersion = _requestTemplatesVersion;
        }

        if (_mockTemplatesVersion > _mockTemplatesSavedVersion)
        {
            await _appDataRepository.SaveDefaultMockTemplatesAsync(MockTemplates);
            _mockTemplatesSavedVersion = _mockTemplatesVersion;
        }
    }

    /// <inheritdoc />
    public AppSettings AppSettings
    {
        get
        {
            if (_appSettings is null) throw new DataStorageIsNotInitializedException(nameof(_appSettings));
            return _appSettings;
        }
        set
        {
            _appSettings = value;
            _appSettingsVersion++;
        }
    }

    /// <inheritdoc />
    public List<RequestTemplate> RequestTemplates
    {
        get
        {
            if (_requestTemplates is null) throw new DataStorageIsNotInitializedException(nameof(_requestTemplates));
            return _requestTemplates;
        }
        set
        {
            _requestTemplates = value;
            _requestTemplatesVersion++;
        }
    }

    /// <inheritdoc />
    public List<MockTemplate> MockTemplates
    {
        get
        {
            if (_mockTemplates is null) throw new DataStorageIsNotInitializedException(nameof(_requestTemplates));
            return _mockTemplates;
        }
        set
        {
            _mockTemplates = value;
            _mockTemplatesVersion++;
        }
    }
}