using Domain.Enums;
using Domain.Models;

namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// DTO for a <see cref="MockTemplate"/>
/// </summary>
/// <param name="Type">Type of a Mock</param>
internal sealed record MockTemplateDto(
    MockTypes Type
    );