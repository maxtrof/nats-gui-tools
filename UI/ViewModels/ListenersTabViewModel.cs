using System;
using System.Collections.ObjectModel;
using System.Linq;
using Domain.Models;
using ReactiveUI;
using UI.Helpers;
using UI.MessagesBus;

namespace UI.ViewModels;

internal sealed class ListenersTabViewModel : ViewModelBase
{
    private ListenerEditViewModel? _selectedTab;
    public ObservableCollection<ListenerEditViewModel> Tabs { get; set; } = new();

    internal ListenerEditViewModel? SelectedTab
    {
        get => _selectedTab;
        set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
    }

    public ListenersTabViewModel()
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
        var exists = Tabs.FirstOrDefault(x => x.Listener.Id == requestId);
        if (exists is not null)
        {
            exists.Name = name;
            SelectedTab = exists;
        }
    }

    private void AddListenerTab(ListenerEditViewModel? request = null)
    {
        if (request == null)
        {
            request = new ListenerEditViewModel
            {
                Name = $"listener_{NameGenerator.GetRandomName()}"
            };
        }

        var exists = Tabs.FirstOrDefault(x => x.Name == request.Name);
        if (exists == null)
            Tabs.Add(request);
        SelectedTab = exists ?? request;
    }

    private void SubscribeToMessageBus()
    {
        MessageBus.Current.Listen<Listener>(BusEvents.ListenerSelected)
            .Subscribe(listener =>
            {
                var listenerEditViewModel = new ListenerEditViewModel(listener);
                AddListenerTab(listenerEditViewModel);
            });
        MessageBus.Current.Listen<Listener>(BusEvents.ListenerDeleted)
            .Subscribe(request =>
            {
                var exists = Tabs.FirstOrDefault(x => x.ListenerId == request.Id);
                if (exists is not null)
                    Tabs.Remove(exists);
                SelectedTab = Tabs.FirstOrDefault();
            });
    }
}