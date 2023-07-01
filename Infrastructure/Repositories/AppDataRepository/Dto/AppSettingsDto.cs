using Domain.Models;

namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// DTO for <see cref="AppSettings"/>
/// </summary>
/// <param name="Servers">Server list</param>
/// <param name="UserDictionary">User's dictionary</param>
/// <param name="FormatJson">Should we try format JSON responses from NATS</param>
/// <param name="AutoCompletionDictionary">Keeps last N topic names to provide auto completion</param>
/// <param name="Version">Version of a format</param>
internal sealed record AppSettingsDto(
    List<NatsServerSettingsDto> Servers,
    Dictionary<string, string> UserDictionary,
    bool FormatJson,
    List<string> AutoCompletionDictionary,
    int Version
);