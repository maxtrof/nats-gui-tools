namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// DTO for Mock Templates containing file
/// </summary>
/// <param name="Templates">Mock templates DTOs</param>
internal sealed record MockTemplatesDto(List<MockTemplateDto> Templates);