using System;
using System.Collections.ObjectModel;
using System.Linq;
using Domain.Models;
using ReactiveUI;
using UI.Helpers;
using UI.MessagesBus;

namespace UI.ViewModels;

internal sealed class RequestsTabViewModel : ViewModelBase
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
        SubscribeToMessageBus();
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
            request = new RequestEditViewModel()
            {
                Name = $"request_{NameGenerator.GetRandomName()}"
            };
        }

        var exists = Tabs.FirstOrDefault(x => x.Name == request.Name);
        if (exists == null)
            Tabs.Add(request);
        SelectedTab = exists ?? request;
    }

    private void SubscribeToMessageBus()
    {
        MessageBus.Current.Listen<RequestTemplate>(BusEvents.RequestSelected)
            .Subscribe(request =>
            {
                var requestEditViewModel = new RequestEditViewModel(request);
                AddRequestTab(requestEditViewModel);
            });
        MessageBus.Current.Listen<RequestTemplate>(BusEvents.RequestDeleted)
            .Subscribe(request =>
            {
                var exists = Tabs.FirstOrDefault(x => x.RequestId == request.Id);
                if (exists is not null)
                    Tabs.Remove(exists);
                SelectedTab = Tabs.FirstOrDefault();
            });
    }
}