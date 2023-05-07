using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application;
using Autofac;
using Domain.Models;
using ReactiveUI;

namespace UI.ViewModels;

public class ServerListItemViewModel : ViewModelBase
{
    private readonly ILifetimeScope _scope;
    private readonly ConnectionManager _connectionManager; 
    private NatsServerSettings _serverSettings;
    private bool _isConnected;
    private string _buttonText;

    public ServerListItemViewModel()
    {
        _scope = Program.Container.BeginLifetimeScope();
        _connectionManager = _scope.Resolve<ConnectionManager>();
    }

    public NatsServerSettings ServerSettings
    {
        get => _serverSettings;
        set => this.RaiseAndSetIfChanged(ref _serverSettings, value);
    }

    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            ButtonText = value
                ? "Disconnect"
                : "Connect";
            this.RaiseAndSetIfChanged(ref _isConnected, value);
        }
    }

    public string ButtonText
    {
        get => _buttonText;
        set => this.RaiseAndSetIfChanged(ref _buttonText, value);
    }

    public async Task<bool> ConnectOrDisconnectToAServer()
    {
        
        try
        {
            if (IsConnected)
            {
                await _connectionManager.Disconnect();
            }
            else
            {
                await _connectionManager.Connect(ServerSettings);
            }

            return true;
        }
        catch (Exception ex)
        {
            ButtonText = "Error";
            return false;
        }
    }
}

public static class ServerListItemViewModelMapper 
{
    private static ServerListItemViewModel ToViewModel(this NatsServerSettings settings, string connectedServer)
    {
        return new ServerListItemViewModel
        {
            ServerSettings = settings,
            IsConnected = settings.Address == connectedServer
        };
    }

    public static List<ServerListItemViewModel> ToViewModel(this IEnumerable<NatsServerSettings> list, string connectedServer)
    {
        return list.Select(x => x.ToViewModel(connectedServer)).ToList();
    }
}