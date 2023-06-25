using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Text.RegularExpressions;
using Avalonia.Data;
using Domain.Models;
using ReactiveUI;

namespace UI.ViewModels;

internal sealed class AddOrUpdateServerViewModel : ViewModelBase
{
    private const string NoSpacesRegex = @"^\S*$";
    
    private string _serverName = default!;
    private string _address = default!;
    private int? _port;
    private string? _login;
    private string? _password;
    private bool _tls;
    private bool _addButtonEnabled;
    private bool _isUpdate;
    public Guid Id { get; set; }
    public ReactiveCommand<Unit, NatsServerSettings?> AddOrUpdateServerCommand { get; }

    public AddOrUpdateServerViewModel()
    {
        AddOrUpdateServerCommand = ReactiveCommand.Create(() => new NatsServerSettings
        {
            Name = ServerName,
            Address = Address,
            Login = Login,
            Port = _port,
            Password = Password,
            Tls = Tls
        })!;
    }

    public bool IsUpdate
    {
        get => _isUpdate;
        set => this.RaiseAndSetIfChanged(ref _isUpdate, value);
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
    public string Port
    {
        get => _port.ToString() ?? "";
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                _port = null;
                return;
            }

            if (!int.TryParse(value, out var parsed))
            {
                _port = null;
                throw new DataValidationException("Port should be a valid number (value will be ignored)");
            }
            this.RaiseAndSetIfChanged(ref _port, parsed);
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
               && _port is null or > 0 and <= 65535;
    }
}