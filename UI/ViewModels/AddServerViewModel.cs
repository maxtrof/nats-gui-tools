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


    /// <summary>
    /// Server name
    /// </summary>
    [Required]
    public string ServerName
    {
        get => _serverName;
        set => this.RaiseAndSetIfChanged(ref _serverName, value);
    }

    /// <summary>
    /// Server IP or name
    /// </summary>
    [Required]
    public string Address
    {
        get => _address;
        set => this.RaiseAndSetIfChanged(ref _address, value);
    }

    /// <summary>
    /// Port if provided
    /// </summary>
    [Range(0, 65535)]
    public int? Port
    {
        get => _port;
        set => this.RaiseAndSetIfChanged(ref _port, value);
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
}