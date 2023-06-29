using Domain.Enums;
using Domain.Models;

namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// DTO for a <see cref="MockTemplate"/>
/// </summary>
/// <param name="Name">Name of a Mock</param>
/// <param name="Type">Type of a Mock</param>
/// <param name="Topic">Topic for mock</param>
/// <param name="AnswerTemplate">Answer body</param>
internal sealed record MockTemplateDto(
    string Name,
    MockTypes Type,
    string? Topic,
    string AnswerTemplate);