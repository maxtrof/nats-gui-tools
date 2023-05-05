using Domain.Models;

namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// DTO for <see cref="AppSettings"/>
/// </summary>
/// <param name="Servers">Server list</param>
/// <param name="UserDictionary">User's dictionary</param>
/// <param name="Version">Version of a format</param>
internal sealed record AppSettingsDto(
    List<NatsServerSettingsDto> Servers,
    Dictionary<string, string> UserDictionary,
    int Version
);