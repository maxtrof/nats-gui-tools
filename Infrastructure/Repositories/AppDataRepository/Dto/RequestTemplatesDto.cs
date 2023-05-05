namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// DTO for Request Templates containing file
/// </summary>
/// <param name="Templates">Templates</param>
/// <param name="Version">Version of a format</param>
internal sealed record RequestTemplatesDto(List<RequestTemplateDto> Templates, int Version);