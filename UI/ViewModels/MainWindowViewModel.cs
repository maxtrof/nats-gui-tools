using System.Collections.Generic;
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
using UI.Helpers;
using UI.MessagesBus;
using UI.PeriodicTasks;

namespace UI.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly ILifetimeScope _scope;
    private readonly IDataStorage _storage;
    private readonly ConnectionManager _connectionManager;
    private string _searchText = "";
    private bool _appLoaded;
    private bool _isRequestsTabVisible;
    private bool _isListenersTabVisible;
    private bool _isServersTabVisible = true;
    private int _selectedTab;
    private RequestTemplate _selectedRequest;
    private Listener _selectedListener;

    public ICommand AddNewServer { get; }
    public ICommand ShowSettingsWindow { get; }
    public ICommand ShowExportDialog { get; }
    public ICommand ShowImportDialog { get; }
    public ICommand AddNewRequest { get; }
    public ICommand DeleteRequest { get; }
    public ICommand AddNewListener { get; }
    public ICommand DeleteListener { get; }
    public Interaction<AddServerViewModel, NatsServerSettings?> ShowAddNewServerDialog { get; }
    public Interaction<SettingsViewModel, Dictionary<string, string>?> ShowSettingsWindowDialog { get; }
    public Interaction<Unit, string?> ShowExportFileSaveDialog { get; }
    public Interaction<Unit, string?> ShowImportFileLoadDialog { get; }
    public Interaction<YesNoDialogViewModel, DialogResult> YesNoDialog { get; } = new();
    public ObservableCollection<RequestTemplate> RequestTemplates { get; set; } = new(new List<RequestTemplate>());
    public ObservableCollection<Listener> Listeners { get; set; } = new(new List<Listener>());

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
            IsRequestsTabVisible = value == 2;
            IsListenersTabVisible = value == 1;
            IsServersTabVisible = value == 0;
        }
    }

    public MainWindowViewModel()
    {
        _scope = Program.Container.BeginLifetimeScope();
        _storage = _scope.Resolve<IDataStorage>();
        _connectionManager = _scope.Resolve<ConnectionManager>();
        
        RxApp.MainThreadScheduler.Schedule(LoadData);

        DataSaver = new DataSaver(_storage);

        // Add new Server
        ShowAddNewServerDialog = new Interaction<AddServerViewModel, NatsServerSettings?>();
        
        AddNewServer = ReactiveCommand.CreateFromTask(async () =>
        {
            var vm = new AddServerViewModel();
            var result = await ShowAddNewServerDialog.Handle(vm);
            if (result is null) return;
            _storage.AppSettings.Servers.Add(result);
            _storage.IncAppSettingsVersion();
            SearchText = SearchText; // Force update search to fetch changes in UI
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
            var result = await ShowExportFileSaveDialog.Handle(new Unit());
            if (result is null) return;
            await _storage.ExportAsync(result);
        });
        ShowImportFileLoadDialog = new Interaction<Unit, string?>();
        ShowImportDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await ShowImportFileLoadDialog.Handle(new Unit());
            if (result is null) return;
            await _storage.ImportAsync(result);
            SearchText = SearchText; // Force update search to fetch changes in UI
        });
        
        AddNewRequest = ReactiveCommand.Create(() =>
        {
            var newRequest = new RequestTemplate
            {
                Name = $"Request {NameGenerator.GetRandomName()}"
            };
            _storage.RequestTemplates.Add(newRequest);
            _storage.IncRequestTemplatesVersion();
            RequestTemplates.Add(newRequest);
            MessageBus.Current.SendMessage(newRequest, BusEvents.RequestSelected);
        });

        DeleteRequest = ReactiveCommand.CreateFromTask<RequestTemplate>(async requestTemplate =>
        {
            var result = await YesNoDialog.Handle(new YesNoDialogViewModel()
            {
                Title = "Delete?",
                Text = "Request will be removed permanently?"
            });

            if (result.Result == DialogResultEnum.Yes)
            {
                RequestTemplates.Remove(requestTemplate);
                _storage.RequestTemplates.Remove(requestTemplate);
                _storage.IncRequestTemplatesVersion();
                MessageBus.Current.SendMessage(requestTemplate, BusEvents.RequestDeleted);
            }
        });

        AddNewListener = ReactiveCommand.Create(() =>
        {
            var newListener = new Listener
            {
                Name = $"Listener {NameGenerator.GetRandomName()}"
            };
            _storage.Listeners.Add(newListener);
            _storage.IncListenersVersion();
            Listeners.Add(newListener);
            MessageBus.Current.SendMessage(newListener, BusEvents.ListenerSelected);
        });

        DeleteListener = ReactiveCommand.CreateFromTask<Listener>(async listener =>
        {
            var result = await YesNoDialog.Handle(new YesNoDialogViewModel()
            {
                Title = "Delete?",
                Text = "Listener will be removed permanently?"
            });

            if (result.Result == DialogResultEnum.Yes)
            {
                Listeners.Remove(listener);
                _storage.Listeners.Remove(listener);
                _storage.IncListenersVersion();
                MessageBus.Current.SendMessage(listener, BusEvents.RequestDeleted);
            }
        });

        MessageBus.Current.Listen<RequestTemplate>(BusEvents.RequestUpdated)
            .Subscribe(requestTemplate =>
            {
                var exists = RequestTemplates.FirstOrDefault(x => x.Id == requestTemplate.Id);
                if (exists is null)
                    return;

                RequestTemplates.Replace(exists, requestTemplate);
                _storage.RequestTemplates = RequestTemplates.ToList();
                _storage.IncRequestTemplatesVersion();
            });

        MessageBus.Current.Listen<Listener>(BusEvents.ListenerUpdated)
            .Subscribe(listeners =>
            {
                var exists = Listeners.FirstOrDefault(x => x.Id == listeners.Id);
                if (exists is null)
                    return;

                Listeners.Replace(exists, listeners);
                _storage.Listeners = Listeners.ToList();
                _storage.IncRequestTemplatesVersion();
            });
    }

    /// <summary>
    /// Periodic data saver
    /// </summary>
    public DataSaver DataSaver { get; set; }


    /// <summary>
    /// Unified search field for different sections (Requests, Mocks, etc.)
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            this.RaiseAndSetIfChanged(ref _searchText, value);
            UpdateServersList();
        }
    }

    public void UpdateServersList()
    {
        Servers.Clear();
        var searchRequestInLower = SearchText.ToLower();
        // Filter servers
        Servers.AddRange(string.IsNullOrWhiteSpace(SearchText)
            ? _storage.AppSettings.Servers.ToViewModel(_connectionManager.GetCurrentServerName)
            : _storage.AppSettings.Servers.Where(x => x.Name.ToLower().Contains(searchRequestInLower))
                .ToViewModel(_connectionManager.GetCurrentServerName)
        );
    }

    public bool AppLoaded
    {
        get => _appLoaded;
        set => this.RaiseAndSetIfChanged(ref _appLoaded, value);
    }

    /// <summary>
    /// Servers list
    /// </summary>
    public ObservableCollection<ServerListItemViewModel> Servers { get; set; } = new();

    private async void LoadData()
    {
        await _storage.InitializeAsync();
        AppLoaded = true;
        SearchText = ""; // Force update search to fetch changes in UI
        RequestTemplates.AddRange(_storage.RequestTemplates);
        Listeners.AddRange(_storage.Listeners);
    }
}