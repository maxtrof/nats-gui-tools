using System;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Linq;
using System.Reactive;
using Avalonia.Controls;
using Domain.Models;
using ReactiveUI;

namespace UI.ViewModels;

public class RequestsTabViewModel : ViewModelBase
{
    public ObservableCollection<RequestTab> Tabs { get; set; } = new();
    internal RequestTab? SelectedTab { get; set; }

    public ReactiveCommand<RequestTab, Unit> AddRequestTabCommand { get; }

    public RequestsTabViewModel()
    {
        AddRequestTabCommand = ReactiveCommand.Create<RequestTab>(AddRequestTab);
        Tabs.Add(new RequestTab(new RequestTemplate {Name = "Name 01"}));
        Tabs.Add(new RequestTab(new RequestTemplate {Name = "Name 02"}));
        Tabs.Add(new RequestTab(new RequestTemplate {Name = "Name 03"}));
        Tabs.Add(new RequestTab(new RequestTemplate {Name = "Name 03"}));
        Tabs.Add(new RequestTab(new RequestTemplate {Name = "Name 03"}));
    }

    private void AddRequestTab(RequestTab tab)
    {
        // var exists = Tabs.FirstOrDefault(x => x.Request.Name == tab.Request.Name);
        // if (exists == null)
        //     Tabs.Add(tab);
        // SelectedTab = exists ?? tab;
    }
}

/// <summary>
/// One request tab model
/// </summary>
/// <param name="Request">Request template</param>
public class RequestTab
{
    public string TabTitle => Request.Name;
    public string TabId => Guid.NewGuid().ToString();
    public RequestTemplate Request { get; set; }

    public RequestTab(RequestTemplate request)
    {
        this.Request = request;
    }
}