using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autofac;
using Domain.Interfaces;
using Domain.Models;
using DynamicData;

namespace UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ILifetimeScope _scope;
    private readonly IDataStorage _dataStorage;
    private string _searchText = "";

    public MainWindowViewModel()
    {
        _scope = Program.Container.BeginLifetimeScope();
        _dataStorage = _scope.Resolve<IDataStorage>();
        _dataStorage.InitializeAsync().Wait();
        _dataStorage.RequestTemplates.Add(new RequestTemplate
        {
            Body   = "test",
            Name = "test",
            Topic = "test"
        });
        RequestTemplates.AddRange(_dataStorage.RequestTemplates);
    }
    
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            RequestTemplates.Clear();
            RequestTemplates.AddRange(_dataStorage.RequestTemplates.Where(x => x.Name.Contains(SearchText) || string.IsNullOrWhiteSpace(SearchText)).ToList());
        }
    }

    public ObservableCollection<RequestTemplate> RequestTemplates { get; set; } = new (new List<RequestTemplate>());
}