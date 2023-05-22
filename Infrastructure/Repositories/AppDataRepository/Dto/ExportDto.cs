namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// Export/import DTO
/// </summary>
/// <param name="MockTemplates">Mocks</param>
/// <param name="RequestTemplates">Requests</param>
/// <param name="Version">Format version</param>
internal record ExportDto(
    MockTemplatesDto MockTemplates,
    RequestTemplatesDto RequestTemplates,
    int Version = 1);