using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Text.RegularExpressions;
using Domain.Models;
using ReactiveUI;

namespace UI.ViewModels;

public sealed class AddServerViewModel : ViewModelBase
{
    private const string NoSpacesRegex = @"^\S*$";
    
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
            this.RaiseAndSetIfChanged(ref _serverName, value);
            AddButtonEnabled = IsModelValid();
        }
    }

    /// <summary>
    /// Server IP or name
    /// </summary>
    [Required]
    [RegularExpression(NoSpacesRegex, ErrorMessage = "No white space allowed")]
    public string Address
    {
        get => _address;
        set
        {
            this.RaiseAndSetIfChanged(ref _address, value);
            AddButtonEnabled = IsModelValid();
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
            this.RaiseAndSetIfChanged(ref _port, value);
            AddButtonEnabled = IsModelValid();
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
               && Regex.IsMatch(Address, NoSpacesRegex)
               && Port is null or > 0 and <= 65535;
    }
}