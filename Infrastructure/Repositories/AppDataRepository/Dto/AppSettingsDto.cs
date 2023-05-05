using Domain.Models;

namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// DTO for <see cref="AppSettings"/>
/// </summary>
/// <param name="Servers">Server list</param>
internal sealed record AppSettingsDto(
    List<NatsServerSettingsDto> Servers
);