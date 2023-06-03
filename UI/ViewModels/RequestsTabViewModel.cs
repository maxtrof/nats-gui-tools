using System;
using System.Collections.ObjectModel;
using System.Linq;
using Domain.Models;
using ReactiveUI;

namespace UI.ViewModels;

public class RequestsTabViewModel : ViewModelBase
{
    private RequestEditViewModel? _selectedTab;
    public ObservableCollection<RequestEditViewModel> Tabs { get; set; } = new();

    internal RequestEditViewModel? SelectedTab
    {
        get => _selectedTab;
        set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
    }

    public RequestsTabViewModel()
    {
        MessageBus.Current.Listen<RequestTemplate>("onRequestSelected")
            .Subscribe(request =>
            {
                var requestEditViewModel = new RequestEditViewModel(request);
                AddRequestTab(requestEditViewModel);
            });
    }

    public void SetSelectedTab(string name)
    {
        var exists = Tabs.FirstOrDefault(x => x.Name == name);
        if (exists is not null)
            SelectedTab = exists;
    }
    
    public void UpdateRequestName(Guid requestId, string name)
    {
        var exists = Tabs.FirstOrDefault(x => x.RequestTemplate.Id == requestId);
        if (exists is not null)
        {
            exists.Name = name;
            SelectedTab = exists;
        }
    }

    private void AddRequestTab(RequestEditViewModel? request = null)
    {
        if (request == null)
        {
            var n = 0;
            string newName;
            do
            {
                n++;
                newName = $"New_request_{n}";
            } while (Tabs.Any(x => x.Name == newName));

            request = new RequestEditViewModel()
            {
                Name = newName
            };
        }

        var exists = Tabs.FirstOrDefault(x => x.Name == request.Name);
        if (exists == null)
            Tabs.Add(request);
        SelectedTab = exists ?? request;
    }
}