namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// Listener DTO
/// </summary>
/// <param name="Name">Listener name</param>
/// <param name="Topic">Topic to listen</param>
internal sealed record ListenerDto(string Name, string? Topic);