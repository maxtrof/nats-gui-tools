using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application;
using Autofac;
using Domain.Models;
using ReactiveUI;
using UI.Helpers;

namespace UI.ViewModels;

internal sealed class ServerListItemViewModel : ViewModelBase
{
    private readonly ConnectionManager _connectionManager;
    private NatsServerSettings _serverSettings = default!;
    private bool _isConnected;
    private string _buttonText = string.Empty;

    public Guid Id { get; set; }

    public ServerListItemViewModel()
    {
        var scope = Program.Container.BeginLifetimeScope();
        _connectionManager = scope.Resolve<ConnectionManager>();
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
            ErrorHelper.ShowError(ex.Message);
            return false;
        }
    }
}

internal static class ServerListItemViewModelMapper 
{
    private static ServerListItemViewModel ToViewModel(this NatsServerSettings settings, string connectedServer)
    {
        return new ServerListItemViewModel
        {
            ServerSettings = settings,
            IsConnected = settings.Address == connectedServer,
            Id = settings.Id
        };
    }

    public static List<ServerListItemViewModel> ToViewModel(this IEnumerable<NatsServerSettings> list, string connectedServer)
    {
        return list.Select(x => x.ToViewModel(connectedServer)).ToList();
    }
}