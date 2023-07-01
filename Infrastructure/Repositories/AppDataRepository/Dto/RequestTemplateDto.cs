using Domain.Enums;
using Domain.Models;

namespace Infrastructure.Repositories.AppDataRepository.Dto;

/// <summary>
/// DTO for a <see cref="RequestTemplate"/>
/// </summary>
/// <param name="Name">Template name</param>
/// <param name="Topic">Topic to send Request</param>
/// <param name="Body">Request body</param>
/// <param name="Body">Request type</param>
internal sealed record RequestTemplateDto(
    string Name,
    string? Topic,
    string Body,
    RequestType Type
    );