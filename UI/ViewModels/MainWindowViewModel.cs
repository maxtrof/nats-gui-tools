using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Windows.Input;
using Application;
using Autofac;
using Domain.Interfaces;
using Domain.Models;
using DynamicData;
using ReactiveUI;

namespace UI.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly ILifetimeScope _scope;
    private readonly IDataStorage _storage;
    private readonly ConnectionManager _connectionManager;
    private string _searchText = "";
    private bool _appLoaded;

    public ICommand AddNewServer { get; }
    public Interaction<AddServerViewModel, NatsServerSettings?> ShowAddNewServerDialog { get; }
    public MainWindowViewModel()
    {
        _scope = Program.Container.BeginLifetimeScope();
        _storage = _scope.Resolve<IDataStorage>();
        _connectionManager = _scope.Resolve<ConnectionManager>();
        
        RxApp.MainThreadScheduler.Schedule(LoadData);

        ShowAddNewServerDialog = new Interaction<AddServerViewModel, NatsServerSettings?>();
        AddNewServer = ReactiveCommand.CreateFromTask(async () =>
        {
            var vm = new AddServerViewModel();
            var result = await ShowAddNewServerDialog.Handle(vm);
            if (result is null) return;
            _storage.AppSettings.Servers.Add(result);
            SearchText = SearchText; // Force update search to fetch changes in UI
        });
    }
    
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
            : _storage.AppSettings.Servers.Where(x => x.Name.ToLower().Contains(searchRequestInLower)).ToViewModel(_connectionManager.GetCurrentServerName)
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
    public ObservableCollection<RequestTemplate> RequestTemplates { get; set; } = new (new List<RequestTemplate>());

    private async void LoadData()
    {
        await _storage.InitializeAsync();
        AppLoaded = true;
    }
}