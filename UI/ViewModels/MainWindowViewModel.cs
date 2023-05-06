using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
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
    private string _searchText = "";
    
    public ICommand AddNewServer { get; }
    public Interaction<AddServerViewModel, NatsServerSettings?> ShowAddNewServerDialog { get; }
    public MainWindowViewModel()
    {
        _scope = Program.Container.BeginLifetimeScope();

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

    public ObservableCollection<RequestTemplate> RequestTemplates { get; set; } = new (new List<RequestTemplate>());
}