using System.ComponentModel.DataAnnotations;
using System.Reactive;
using Domain.Models;
using ReactiveUI;

namespace UI.ViewModels;

public sealed class AddServerViewModel : ViewModelBase
{
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
    private string ServerName { get; set; } = default!;
    /// <summary>
    /// Server IP or name
    /// </summary>
    [Required]
    private string Address { get; set; } = default!;
    /// <summary>
    /// Port if provided
    /// </summary>
    private int? Port { get; set; }
    /// <summary>
    /// Username if provided
    /// </summary>
    private string? Login { get; set; }
    /// <summary>
    /// Password if provided
    /// </summary>
    private string? Password { get; set; }
    /// <summary>
    /// Should use secure connection
    /// </summary>
    private bool Tls { get; set; } 
}