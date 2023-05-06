using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Windows.Input;
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
    private string _searchText = "";
    private bool _appLoaded;

    public ICommand AddNewServer { get; }
    public Interaction<AddServerViewModel, NatsServerSettings?> ShowAddNewServerDialog { get; }
    public MainWindowViewModel()
    {
        _scope = Program.Container.BeginLifetimeScope();
        _storage = _scope.Resolve<IDataStorage>();
        
        RxApp.MainThreadScheduler.Schedule(LoadData);

        ShowAddNewServerDialog = new Interaction<AddServerViewModel, NatsServerSettings?>();
        AddNewServer = ReactiveCommand.CreateFromTask(async () =>
        {
            var vm = new AddServerViewModel();
            var result = await ShowAddNewServerDialog.Handle(vm);
            Console.Write(result?.Name);
        });
    }
    
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
        }
    }

    public bool AppLoaded
    {
        get => _appLoaded;
        set => this.RaiseAndSetIfChanged(ref _appLoaded, value);
    }

    public ObservableCollection<RequestTemplate> RequestTemplates { get; set; } = new (new List<RequestTemplate>());

    private async void LoadData()
    {
        await _storage.InitializeAsync();
        AppLoaded = true;
    }
}