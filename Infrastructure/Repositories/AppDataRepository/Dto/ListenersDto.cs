namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// Collection of Listeners (DTO)
/// </summary>
/// <param name="Listeners">Listeners</param>
/// <param name="Version">Format version</param>
internal sealed record ListenersDto(List<ListenerDto> Listeners, int Version);

