using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using ReactiveUI;

namespace UI.ViewModels;

public class ServerListItemViewModel : ViewModelBase
{
    private string _name;
    private bool _isConnected;

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public bool IsConnected
    {
        get => _isConnected;
        set => this.RaiseAndSetIfChanged(ref _isConnected, value);
    }
}

public static class ServerListItemViewModelMapper 
{
    private static ServerListItemViewModel ToViewModel(this NatsServerSettings settings, string connectedServer)
    {
        return new ServerListItemViewModel
        {
            Name = settings.Name,
            IsConnected = settings.Name == connectedServer
        };
    }

    public static List<ServerListItemViewModel> ToViewModel(this IEnumerable<NatsServerSettings> list, string connectedServer)
    {
        return list.Select(x => x.ToViewModel(connectedServer)).ToList();
    }
}