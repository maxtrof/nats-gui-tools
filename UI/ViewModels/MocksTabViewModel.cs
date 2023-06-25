using System;
using System.Collections.ObjectModel;
using System.Linq;
using Domain.Models;
using ReactiveUI;
using UI.Helpers;
using UI.MessagesBus;

namespace UI.ViewModels;

internal sealed class MocksTabViewModel : ViewModelBase
{
    private MockEditViewModel? _selectedTab;
    public ObservableCollection<MockEditViewModel> Tabs { get; set; } = new();

    internal MockEditViewModel? SelectedTab
    {
        get => _selectedTab;
        set => this.RaiseAndSetIfChanged(ref _selectedTab, value);
    }

    public MocksTabViewModel()
    {
        SubscribeToMessageBus();
    }

    public void SetSelectedTab(string name)
    {
        var exists = Tabs.FirstOrDefault(x => x.Name == name);
        if (exists is not null)
            SelectedTab = exists;
    }

    private void SubscribeToMessageBus()
    {
        MessageBus.Current.Listen<MockTemplate>(BusEvents.MockSelected)
            .Subscribe(mock =>
            {
                var mockEditViewModel = new MockEditViewModel(mock);
                AddMockTab(mockEditViewModel);
            });
        MessageBus.Current.Listen<MockTemplate>(BusEvents.MockDeleted)
            .Subscribe(mock =>
            {
                var exists = Tabs.FirstOrDefault(x => x.MockId == mock.Id);
                if (exists is not null)
                    Tabs.Remove(exists);
                SelectedTab = Tabs.FirstOrDefault();
            });
    }

    public void UpdateMockName(Guid MockId, string name)
    {
        var exists = Tabs.FirstOrDefault(x => x.MockTemplate.Id == MockId);
        if (exists is not null)
        {
            exists.Name = name;
            SelectedTab = exists;
        }
    }

    private void AddMockTab(MockEditViewModel? Mock = null)
    {
        if (Mock == null)
        {
            Mock = new MockEditViewModel()
            {
                Name = $"Mock_{NameGenerator.GetRandomName()}"
            };
        }

        var exists = Tabs.FirstOrDefault(x => x.Name == Mock.Name);
        if (exists == null)
            Tabs.Add(Mock);
        SelectedTab = exists ?? Mock;
    }
}