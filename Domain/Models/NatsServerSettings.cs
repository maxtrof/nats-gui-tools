namespace Domain.Models;

public sealed class NatsServerSettings
{
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string? Port { get; set; } = default!;
    public string? Login { get; set; } = default!;
    public string? Password { get; set; } = default!;
}