namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// DTO for Mock Templates containing file
/// </summary>
/// <param name="Templates">Mock templates DTOs</param>
/// <param name="Version">Version of a format</param>
internal sealed record MockTemplatesDto(List<MockTemplateDto> Templates, int Version);