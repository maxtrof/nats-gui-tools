using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive;
using Domain.Models;
using ReactiveUI;

namespace UI.ViewModels;

public sealed class AddServerViewModel : ViewModelBase
{
    private string _serverName = default!;
    private string _address = default!;
    private int? _port;
    private string? _login;
    private string? _password;
    private bool _tls;
    private bool _addButtonEnabled;
    public ReactiveCommand<Unit, NatsServerSettings?> AddServerCommand { get; }

    public AddServerViewModel()
    {
        AddServerCommand = ReactiveCommand.Create(() => new NatsServerSettings
        {
            Name = ServerName,
            Address = Address,
            Login = Login,
            Port = Port,
            Password = Password,
            Tls = Tls
        })!;
    }

    public bool AddButtonEnabled
    {
        get => _addButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _addButtonEnabled, value);
    }

    /// <summary>
    /// Server name
    /// </summary>
    [Required]
    public string ServerName
    {
        get => _serverName;
        set
        {
            AddButtonEnabled = IsModelValid();
            this.RaiseAndSetIfChanged(ref _serverName, value);
        }
    }

    /// <summary>
    /// Server IP or name
    /// </summary>
    [Required]
    public string Address
    {
        get => _address;
        set
        {
            AddButtonEnabled = IsModelValid();
            this.RaiseAndSetIfChanged(ref _address, value);
        }
    }

    /// <summary>
    /// Port if provided
    /// </summary>
    [Range(0, 65535)]
    public int? Port
    {
        get => _port;
        set
        {
            AddButtonEnabled = IsModelValid();
            this.RaiseAndSetIfChanged(ref _port, value);
        }
    }

    /// <summary>
    /// Username if provided
    /// </summary>
    public string? Login
    {
        get => _login;
        set => this.RaiseAndSetIfChanged(ref _login, value);
    }

    /// <summary>
    /// Password if provided
    /// </summary>
    public string? Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    /// <summary>
    /// Should use secure connection
    /// </summary>
    public bool Tls
    {
        get => _tls;
        set => this.RaiseAndSetIfChanged(ref _tls, value);
    }

    private bool IsModelValid()
    {
        return !string.IsNullOrWhiteSpace(ServerName)
               && !string.IsNullOrWhiteSpace(Address)
               && Port is null or > 0 and <= 65535;
    }
}