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
    public ObservableCollection<RequestEditViewModel> Tabs { get; set; } = new();
    internal RequestEditViewModel? SelectedTab { get; set; }

    public RequestsTabViewModel()
    {
        MessageBus.Current.Listen<string>("onRequestSelected")
            .Subscribe(name =>
            {
                var requestEditViewModel = new RequestEditViewModel(name);
                AddRequestTab(requestEditViewModel);
            });
    }

    private void AddRequestTab(RequestEditViewModel? request = null)
    {
        if (request == null)
        {
            int n = 0;
            string newName;
            do
            {
                n++;
                newName = $"New_request_{n}";
            } while (Tabs.Any(x => x.Name == newName));

            request = new RequestEditViewModel(newName);
        }

        var exists = Tabs.FirstOrDefault(x => x.Name == request.Name);
        if (exists == null)
            Tabs.Add(request);
        SelectedTab = exists ?? request;
    }
}