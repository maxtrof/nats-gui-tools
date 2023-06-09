﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Windows.Input;
using Application;
using Autofac;
using Domain.Interfaces;
using Domain.Models;
using DynamicData;
using ReactiveUI;
using System;
using DynamicData.Binding;
using UI.Helpers;
using UI.MessagesBus;
using UI.PeriodicTasks;
using System.Threading.Tasks;

namespace UI.ViewModels;

internal sealed class MainWindowViewModel : ViewModelBase
{
    private readonly ILifetimeScope _scope;
    private readonly IDataStorage _storage;
    private readonly ConnectionManager _connectionManager;
    private string _searchText = "";
    private bool _appLoaded;
    private bool _isRequestsTabVisible;
    private bool _isListenersTabVisible;
    private bool _isMocksTabVisible;
    private bool _isServersTabVisible = true;
    private bool _formatJson;
    private int _selectedTab;
    private string? _errorMessage;
    private RequestTemplate _selectedRequest = default!;
    private Listener _selectedListener = default!;
    private MockTemplate _selectedMockTemplate = default!; 


    public ICommand AddNewServer { get; }
    public ICommand EditServer { get; }
    public ICommand DeleteServer { get; }
    public ICommand ShowSettingsWindow { get; }
    public ICommand ShowExportDialog { get; }
    public ICommand ShowImportDialog { get; }
    public ICommand AddNewRequest { get; }
    public ICommand DeleteRequest { get; }
    public ICommand AddNewListener { get; }
    public ICommand DeleteListener { get; }
    public ICommand AddNewMock { get; }
    public ICommand DeleteMock { get; }
    public Interaction<AddOrUpdateServerViewModel, NatsServerSettings?> ShowAddOrUpdateServerDialog { get; }
    public Interaction<SettingsViewModel, Dictionary<string, string>?> ShowSettingsWindowDialog { get; }
    public Interaction<Unit, string?> ShowExportFileSaveDialog { get; }
    public Interaction<Unit, string?> ShowImportFileLoadDialog { get; }
    public Interaction<YesNoDialogViewModel, DialogResult> YesNoDialog { get; } = new();
    
    // Source caches for lists
    public SourceCache<RequestTemplate, Guid> _requestTemplates { get; set; } = new(x => x.Id);
    public SourceCache<Listener, Guid> _listeners { get; set; } = new(x => x.Id);
    public SourceCache<MockTemplate, Guid> _mocks { get; set; } = new(x => x.Id);
    public SourceCache<ServerListItemViewModel, Guid> _servers { get; set; } = new(x => x.Id);
    
    // Filtered data for lists
    private readonly ReadOnlyObservableCollection<RequestTemplate> _requestTemplatesFiltered;
    public ReadOnlyObservableCollection<RequestTemplate> RequestTemplates => _requestTemplatesFiltered;
    
    // Filtered data for lists
    private readonly ReadOnlyObservableCollection<MockTemplate> _mockTemplatesFiltered;
    public ReadOnlyObservableCollection<MockTemplate> MockTemplates => _mockTemplatesFiltered;
    
    
    private readonly ReadOnlyObservableCollection<ServerListItemViewModel> _serversFiltered;
    public ReadOnlyObservableCollection<ServerListItemViewModel> Servers => _serversFiltered;
    

    private readonly ReadOnlyObservableCollection<Listener> _listenersFiltered;
    public ReadOnlyObservableCollection<Listener> Listeners => _listenersFiltered;
    
    /// <summary>
    /// Periodic data saver
    /// </summary>
    public DataSaver DataSaver { get; set; }
    
    /// <summary>
    /// Connection stats updater
    /// </summary>
    public ConnectionStatsUpdater StatsUpdater { get; set; }

    public RequestTemplate SelectedRequest
    {
        get => _selectedRequest;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedRequest, value);
            MessageBus.Current.SendMessage(SelectedRequest, BusEvents.RequestSelected);
        }
    }
    
    public Listener SelectedListener
    {
        get => _selectedListener;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedListener, value);
            MessageBus.Current.SendMessage(SelectedListener, BusEvents.ListenerSelected);
        }
    }

    public MockTemplate SelectedMockTemplate
    {
        get => _selectedMockTemplate;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedMockTemplate, value);
            MessageBus.Current.SendMessage(SelectedMockTemplate, BusEvents.MockSelected);
        }
    }

    public bool IsRequestsTabVisible
    {
        get => _isRequestsTabVisible;
        set => this.RaiseAndSetIfChanged(ref _isRequestsTabVisible, value);
    }
    
    public bool IsListenersTabVisible
    {
        get => _isListenersTabVisible;
        set => this.RaiseAndSetIfChanged(ref _isListenersTabVisible, value);
    }
    public bool IsMocksTabVisible
    {
        get => _isMocksTabVisible;
        set => this.RaiseAndSetIfChanged(ref _isMocksTabVisible, value);
    }
    
    public bool IsServersTabVisible
    {
        get => _isServersTabVisible;
        set => this.RaiseAndSetIfChanged(ref _isServersTabVisible, value);
    }

    public int SelectedTab
    {
        get => _selectedTab;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedTab, value);
            IsServersTabVisible = value == 0;
            IsListenersTabVisible = value == 1;
            IsRequestsTabVisible = value == 2;
            IsMocksTabVisible = value == 3;
        }
    }
    
    /// <summary>
    /// Unified search field for different sections (Requests, Mocks, etc.)
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    /// <summary>
    /// Global error message
    /// </summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    /// <summary>
    /// If true - we'll try to format JSON responses/subscriptions
    /// </summary>
    public bool FormatJson
    {
        get => _formatJson;
        set
        {
            this.RaiseAndSetIfChanged(ref _formatJson, value);
            if (_storage.AppSettings.FormatJson != value)
            {
                _storage.AppSettings.FormatJson = value;
                _storage.IncAppSettingsVersion();   
            }
        }
    }

    /// <summary>
    /// True if app is loaded
    /// </summary>
    public bool AppLoaded
    {
        get => _appLoaded;
        set => this.RaiseAndSetIfChanged(ref _appLoaded, value);
    }

    public MainWindowViewModel()
    {
        _scope = Program.Container.BeginLifetimeScope();
        _storage = _scope.Resolve<IDataStorage>();
        _connectionManager = _scope.Resolve<ConnectionManager>();

        RxApp.MainThreadScheduler.Schedule(LoadData);

        _storage = _storage ?? throw new ArgumentNullException(nameof(_storage));
        DataSaver = new DataSaver(_storage);
        StatsUpdater = new ConnectionStatsUpdater(_connectionManager);
        
        // Search
        _servers.Connect()
            .AutoRefreshOnObservable(_ => this.ObservableForProperty(x => x.SearchText))
            .Filter(x =>
                string.IsNullOrWhiteSpace(SearchText) || x.ServerSettings.Name.ToLower().Contains(SearchText.ToLower()))
            .Sort(SortExpressionComparer<ServerListItemViewModel>.Ascending(t => t.ServerSettings.Name))
            .Bind(out _serversFiltered)
            .Subscribe();
        _listeners.Connect()
            .AutoRefreshOnObservable(_ => this.ObservableForProperty(x => x.SearchText))
            .Filter(x => string.IsNullOrWhiteSpace(SearchText) || x.Name.ToLower().Contains(SearchText.ToLower()))
            .Sort(SortExpressionComparer<Listener>.Ascending(t => t.Name))
            .Bind(out _listenersFiltered)
            .Subscribe();
        _requestTemplates.Connect()
            .AutoRefreshOnObservable(_ => this.ObservableForProperty(x => x.SearchText))
            .Filter(x => string.IsNullOrWhiteSpace(SearchText) || x.Name.ToLower().Contains(SearchText.ToLower()))
            .Sort(SortExpressionComparer<RequestTemplate>.Ascending(t => t.Name))
            .Bind(out _requestTemplatesFiltered)
            .Subscribe();
        _mocks.Connect()
            .AutoRefreshOnObservable(_ => this.ObservableForProperty(x => x.SearchText))
            .Filter(x => string.IsNullOrWhiteSpace(SearchText) || x.Name.ToLower().Contains(SearchText.ToLower()))
            .Sort(SortExpressionComparer<MockTemplate>.Ascending(t => t.Name))
            .Bind(out _mockTemplatesFiltered)
            .Subscribe();

        // Add new Server
        ShowAddOrUpdateServerDialog = new Interaction<AddOrUpdateServerViewModel, NatsServerSettings?>();
        
        AddNewServer = ReactiveCommand.CreateFromTask(async () =>
        {
            var vm = new AddOrUpdateServerViewModel() {
                Id = Guid.NewGuid()
            };
            var result = await ShowAddOrUpdateServerDialog.Handle(vm);
            if (result is null) return;
            _storage.AppSettings.Servers.Add(result);
            _storage.IncAppSettingsVersion();
            UpdateServersList();
        });
        
        // EditServer
        EditServer = ReactiveCommand.CreateFromTask(async (Guid id) =>
        {
            var server = _storage.AppSettings.Servers.First(x => x.Id == id);
            var vm = new AddOrUpdateServerViewModel
            {
                IsUpdate = true,
                Address = server.Address,
                Login = server.Login,
                Password = server.Password,
                Port = server.Port?.ToString() ?? string.Empty,
                ServerName = server.Name,
                Tls = server.Tls
            };
            var result = await ShowAddOrUpdateServerDialog.Handle(vm);
            if (result is null) return;
            result.Id = id;
            _storage.AppSettings.Servers.Replace(server, result);
            _storage.IncAppSettingsVersion();
            UpdateServersList();
        });
        
        // Delete server
        DeleteServer = ReactiveCommand.CreateFromTask(async (Guid id) =>
        {
            var result = await YesNoDialog.Handle(new YesNoDialogViewModel()
            {
                Title = "Delete?",
                Text = "Server will be removed permanently"
            });
            if (result.Result == DialogResultEnum.Yes)
            {
                var server = _storage.AppSettings.Servers.First(x => x.Id == id);
                _storage.AppSettings.Servers.Remove(server);
                _storage.IncAppSettingsVersion();
                UpdateServersList();
            }
        });

        // Settings
        ShowSettingsWindowDialog = new Interaction<SettingsViewModel, Dictionary<string, string>?>();
        ShowSettingsWindow = ReactiveCommand.CreateFromTask(async () =>
        {
            var vm = new SettingsViewModel(_storage.AppSettings.UserDictionary);
            var result = await ShowSettingsWindowDialog.Handle(vm);
            if (result is null) return;
            _storage.AppSettings.UserDictionary = result;
            _storage.IncAppSettingsVersion();
        });

        // Import and export
        ShowExportFileSaveDialog = new Interaction<Unit, string?>();
        ShowExportDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await ShowExportFileSaveDialog.Handle(Unit.Default);
            if (result is null) return;
            await _storage.ExportAsync(result);
        });
        ShowImportFileLoadDialog = new Interaction<Unit, string?>();
        ShowImportDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await ShowImportFileLoadDialog.Handle(Unit.Default);
            if (result is null) return;
            await _storage.ImportAsync(result);
            UpdateListsFromStorage();
        });
        
        AddNewRequest = ReactiveCommand.Create(() =>
        {
            var newRequest = new RequestTemplate
            {
                Name = $"Request {NameGenerator.GetRandomName()}",
                Topic = ""
            };
            _storage.RequestTemplates.Add(newRequest);
            _storage.IncRequestTemplatesVersion();
            _requestTemplates.AddOrUpdate(newRequest);
            MessageBus.Current.SendMessage(newRequest, BusEvents.RequestSelected);
        });

        DeleteRequest = ReactiveCommand.CreateFromTask<RequestTemplate>(async requestTemplate =>
        {
            var result = await YesNoDialog.Handle(new YesNoDialogViewModel()
            {
                Title = "Delete?",
                Text = "Request will be removed permanently"
            });

            if (result.Result == DialogResultEnum.Yes)
            {
                _requestTemplates.Remove(requestTemplate);
                _storage.RequestTemplates.Remove(requestTemplate);
                _storage.IncRequestTemplatesVersion();
                MessageBus.Current.SendMessage(requestTemplate, BusEvents.RequestDeleted);
            }
        });

        AddNewListener = ReactiveCommand.Create(() =>
        {
            var newListener = new Listener
            {
                Name = $"Listener {NameGenerator.GetRandomName()}",
                Topic = ""
            };
            _storage.Listeners.Add(newListener);
            _storage.IncListenersVersion();
            _listeners.AddOrUpdate(newListener);
            MessageBus.Current.SendMessage(newListener, BusEvents.ListenerSelected);
        });

        DeleteListener = ReactiveCommand.CreateFromTask<Listener>(async listener =>
        {
            var result = await YesNoDialog.Handle(new YesNoDialogViewModel()
            {
                Title = "Delete?",
                Text = "Listener will be removed permanently"
            });

            if (result.Result == DialogResultEnum.Yes)
            {
                _listeners.Remove(listener);
                _storage.Listeners.Remove(listener);
                _storage.IncListenersVersion();
                MessageBus.Current.SendMessage(listener, BusEvents.ListenerDeleted);
            }
        });
        
        AddNewMock = ReactiveCommand.Create(() =>
        {
            var newMock = new MockTemplate
            {
                Name = $"Mock {NameGenerator.GetRandomName()}",
                Topic = ""
            };
            _storage.MockTemplates.Add(newMock);
            _storage.IncMockTemplatesVersion();
            _mocks.AddOrUpdate(newMock);
            MessageBus.Current.SendMessage(newMock, BusEvents.MockSelected);
        });

        DeleteMock = ReactiveCommand.CreateFromTask<MockTemplate>(async listener =>
        {
            var result = await YesNoDialog.Handle(new YesNoDialogViewModel()
            {
                Title = "Delete?",
                Text = "Mock will be removed permanently"
            });

            if (result.Result == DialogResultEnum.Yes)
            {
                _mocks.Remove(listener);
                _storage.MockTemplates.Remove(listener);
                _storage.IncMockTemplatesVersion();
                MessageBus.Current.SendMessage(listener, BusEvents.MockDeleted);
            }
        });
        
        MessageBus.Current.Listen<string>(BusEvents.ErrorThrown)
            .Subscribe(text =>
            {
                ErrorMessage = text;
            });

        MessageBus.Current.Listen<RequestTemplate>(BusEvents.RequestUpdated)
            .Subscribe(requestTemplate =>
            {
                var exists = _storage.RequestTemplates.FirstOrDefault(x => x.Id == requestTemplate.Id);
                if (exists is null)
                    return;

                _storage.RequestTemplates.Replace(exists, requestTemplate);
                _requestTemplates.AddOrUpdate(requestTemplate);
                _storage.IncRequestTemplatesVersion();
            });

        MessageBus.Current.Listen<Listener>(BusEvents.ListenerUpdated)
            .Subscribe(listener =>
            {
                var exists = _storage.Listeners.FirstOrDefault(x => x.Id == listener.Id);
                if (exists is null)
                    return;

                _storage.Listeners.Replace(exists, listener);
                _listeners.AddOrUpdate(listener);
                _storage.IncListenersVersion();
            });
        MessageBus.Current.Listen<MockTemplate>(BusEvents.MockUpdated)
            .Subscribe(mockTemplate =>
            {
                var exists = _storage.MockTemplates.FirstOrDefault(x => x.Id == mockTemplate.Id);
                if (exists is null)
                    return;

                _storage.MockTemplates.Replace(exists, mockTemplate);
                _storage.IncMockTemplatesVersion();
                _mocks.AddOrUpdate(mockTemplate);
            });
        MessageBus.Current.Listen<string>(BusEvents.AutocompleteAdded)
            .Subscribe(variant =>
            {
                _storage.AppSettings.AddVariantToAutoCompletionDictionary(variant);
                _storage.IncAppSettingsVersion();
                if (!string.IsNullOrWhiteSpace(variant) && !SharedObservables.Suggestions.Contains(variant))
                    SharedObservables.Suggestions.Add(variant);
            });
    }

    public void UpdateServersList()
    {
        _servers.Clear();
        var serversVms = _storage.AppSettings.Servers.ToViewModel(_connectionManager.GetCurrentServerName);
        foreach (var serverListItemViewModel in serversVms)
        {
            _servers.AddOrUpdate(serverListItemViewModel);
        }
    }

    public void SaveDataAndExit()
    {
        AppLoaded = false;
        var saveDataTask = Task.Run(() => _storage.SaveDataIfNeeded());
        var disconnectTask = Task.Run(() => _connectionManager.Disconnect());
        Task.WaitAll(saveDataTask, disconnectTask);
    }

    private async void LoadData()
    {
        await _storage.InitializeAsync();
        UpdateListsFromStorage();
        FormatJson = _storage.AppSettings.FormatJson;
        
        // Load json highlighting
        JsonHighlighter.LoadJsonHighlighter();
        SharedObservables.Suggestions =
            new ObservableCollection<string>(_storage.AppSettings.GetAutoCompletionDictionary());

        AppLoaded = true;
    }

    private void UpdateListsFromStorage()
    {
        UpdateServersList();
        _requestTemplates.Clear();
        foreach (var storageRequestTemplate in _storage.RequestTemplates)
        {
            _requestTemplates.AddOrUpdate(storageRequestTemplate);
        }
        _listeners.Clear();
        foreach (var storageListener in _storage.Listeners)
        {
            _listeners.AddOrUpdate(storageListener);
        }
        
        _mocks.Clear();
        foreach (var mockTemplate in _storage.MockTemplates)
        {
            _mocks.AddOrUpdate(mockTemplate);
        }
    }
}