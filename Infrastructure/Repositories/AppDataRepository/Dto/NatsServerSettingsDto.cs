using Domain.Models;

namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// DTO for <see cref="NatsServerSettings"/>
/// </summary>
/// <param name="Name">Server name</param>
/// <param name="Address">Server IP or name</param>
/// <param name="Port">Port</param>
/// <param name="Login">Login</param>
/// <param name="Password">Password</param>
internal sealed record NatsServerSettingsDto(
    string Name,
    string Address,
    string? Port,
    string? Login,
    string? Password
);