using Domain.Interfaces;
using Domain.Models;

namespace Application;

/// <summary>
/// App data storage
/// </summary>
public sealed class DataStorage : IDataStorage
{
    private readonly IAppDataRepository _appDataRepository;

    private readonly DataStorageContainer<AppSettings> _appSettings = new ();
    private readonly DataStorageContainer<List<RequestTemplate>> _requestTemplates = new ();
    private readonly DataStorageContainer<List<MockTemplate>> _mockTemplates = new ();
    private readonly DataStorageContainer<List<Listener>> _listeners = new();

    public DataStorage(IAppDataRepository appDataRepository)
    {
        _appDataRepository = appDataRepository ?? throw new ArgumentNullException(nameof(appDataRepository));
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        _appSettings.Data = await _appDataRepository.LoadAppSettingsAsync();
        _requestTemplates.Data = await _appDataRepository.LoadDefaultRequestTemplatesAsync();
        _mockTemplates.Data = await _appDataRepository.LoadDefaultMockTemplatesAsync();
        _listeners.Data = await _appDataRepository.LoadDefaultListenersAsync();
    }

    /// <inheritdoc />
    public async Task SaveDataIfNeeded()
    {
        if (_appSettings.NeedsToBeSaved)
        {
            await _appDataRepository.SaveAppSettingsAsync(_appSettings.Data);
            _appSettings.OnDataSaved();
        }
        if (_requestTemplates.NeedsToBeSaved)
        {
            await _appDataRepository.SaveDefaultRequestTemplatesAsync(_requestTemplates.Data);
            _requestTemplates.OnDataSaved();
        }
        if (_mockTemplates.NeedsToBeSaved)
        {
            await _appDataRepository.SaveDefaultMockTemplatesAsync(_mockTemplates.Data);
            _mockTemplates.OnDataSaved();
        }
        if (_listeners.NeedsToBeSaved)
        {
            await _appDataRepository.SaveDefaultListenersAsync(_listeners.Data);
            _listeners.OnDataSaved();
        }
    }

    /// <inheritdoc />
    public AppSettings AppSettings
    {
        get => _appSettings.Data;
        set => _appSettings.Data = value;
    }

    /// <inheritdoc />
    public List<RequestTemplate> RequestTemplates
    {
        get => _requestTemplates.Data;
        set => _requestTemplates.Data = value;
    }

    /// <inheritdoc />
    public List<MockTemplate> MockTemplates
    {
        get => _mockTemplates.Data;
        set => _mockTemplates.Data = value;
    }

    public List<Listener> Listeners
    {
        get => _listeners.Data;
        set => _listeners.Data = value;
    }

    /// <inheritdoc />
    public void IncAppSettingsVersion() => _appSettings.IncrementVersion();

    /// <inheritdoc />
    public void IncRequestTemplatesVersion() => _requestTemplates.IncrementVersion();

    /// <inheritdoc />
    public void IncMockTemplatesVersion() => _mockTemplates.IncrementVersion();

    /// <inheritdoc />
    public void IncListenersVersion() => _listeners.IncrementVersion();

    /// <inheritdoc />
    public async Task ImportAsync(string fileName)
    {
        var data = await _appDataRepository.Import(fileName);
        _mockTemplates.Data = data.MockTemplates;
        IncMockTemplatesVersion();
        _requestTemplates.Data = data.RequestTemplates;
        IncRequestTemplatesVersion();
        _listeners.Data = data.Listeners;
        IncListenersVersion();
    }

    /// <inheritdoc />
    public Task ExportAsync(string fileName)
    {
        return _appDataRepository.Export(fileName, new Export
        {
            MockTemplates = _mockTemplates.Data,
            RequestTemplates = _requestTemplates.Data,
            Listeners = _listeners.Data
        });
    }
}