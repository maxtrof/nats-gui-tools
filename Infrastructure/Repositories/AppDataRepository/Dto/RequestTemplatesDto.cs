namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// DTO for Request Templates containing file
/// </summary>
/// <param name="Templates">Templates</param>
internal sealed record RequestTemplatesDto(List<RequestTemplateDto> Templates);