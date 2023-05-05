using Domain.Models;
using Infrastructure.Repositories.AppDataRepository.Dto;

namespace Infrastructure.Repositories.AppDataRepository;

/// <summary>
/// Mappings for <see cref="FileStorageAppDataRepository"/>
/// </summary>
internal static class AppDataMappings
{
    /// <summary>
    /// Maps <see cref="AppSettingsDto"/> to <see cref="AppSettings"/>
    /// </summary>
    public static AppSettings ToDomain(this AppSettingsDto dto)
    {
        var settings = new AppSettings();
        settings.Servers = dto.Servers.Select(x => x.ToDomain()).ToList();
        settings.UserDictionary = dto.UserDictionary;
        return settings;
    }

    /// <summary>
    /// Maps <see cref="NatsServerSettingsDto"/> to <see cref="NatsServerSettings"/>
    /// </summary>
    private static NatsServerSettings ToDomain(this NatsServerSettingsDto dto)
    {
        var server = new NatsServerSettings();
        server.Name = dto.Name;
        server.Address = dto.Address;
        server.Port = dto.Port;
        server.Login = dto.Login;
        server.Password = dto.Password;
        server.Tls = dto.Tls;
        return server;
    }

    /// <summary>
    /// Maps <see cref="RequestTemplatesDto"/> to a list of <see cref="RequestTemplate"/>
    /// </summary>
    public static List<RequestTemplate> ToDomain(this RequestTemplatesDto dto)
    {
        var list = new List<RequestTemplate>(dto.Templates.Count);
        foreach (var requestTemplateDto in dto.Templates)
        {
            var template = new RequestTemplate();
            template.Name = requestTemplateDto.Name;
            template.Body = requestTemplateDto.Body;
            template.Topic = requestTemplateDto.Topic;
            list.Add(template);
        }

        return list;
    }
    
    /// <summary>
    /// Maps <see cref="MockTemplatesDto"/> to a list of <see cref="MockTemplate"/>
    /// </summary>
    public static List<MockTemplate> ToDomain(this MockTemplatesDto dto)
    {
        var list = new List<MockTemplate>(dto.Templates.Count);
        foreach (var mockTemplateDto in dto.Templates)
        {
            var template = new MockTemplate();
            template.Type = mockTemplateDto.Type;
            list.Add(template);
        }

        return list;
    }
    
    /// <summary>
    /// Maps <see cref="AppSettingsDto"/> to <see cref="AppSettings"/>
    /// </summary>
    public static AppSettingsDto ToDto(this AppSettings settings)
    {
        var serversDto = settings.Servers.Select(x => x.ToDto()).ToList();
        return new AppSettingsDto(
            Servers: serversDto,
            UserDictionary: settings.UserDictionary
        );
    }

    /// <summary>
    /// Maps <see cref="NatsServerSettings"/> to <see cref="NatsServerSettingsDto"/>
    /// </summary>
    private static NatsServerSettingsDto ToDto(this NatsServerSettings serverSettings)
    {
        return new NatsServerSettingsDto(
            Name: serverSettings.Name,
            Address: serverSettings.Address,
            Port: serverSettings.Port,
            Login: serverSettings.Login,
            Password: serverSettings.Password,
            Tls: serverSettings.Tls
        );
    }

    /// <summary>
    /// Maps a list of <see cref="RequestTemplate"/> to <see cref="RequestTemplatesDto"/>
    /// </summary>
    public static RequestTemplatesDto ToDto(this List<RequestTemplate> templates)
    {
        var templatesDto = new List<RequestTemplateDto>(templates.Count);
        foreach (var requestTemplate in templates)
        {
            var templateDto = new RequestTemplateDto(
                Name: requestTemplate.Name,
                Topic: requestTemplate.Topic,
                Body: requestTemplate.Body
            );
            templatesDto.Add(templateDto);
        }

        return new RequestTemplatesDto(
            Templates: templatesDto
        );
    }
    
    /// <summary>
    /// Maps a list of <see cref="MockTemplate"/> to <see cref="MockTemplatesDto"/>
    /// </summary>
    public static MockTemplatesDto ToDto(this List<MockTemplate> templates)
    {
        var templatesDto = new List<MockTemplateDto>(templates.Count);
        foreach (var mockTemplate in templates)
        {
            var templateDto = new MockTemplateDto(
                Type: mockTemplate.Type
            );
            templatesDto.Add(templateDto);
        }

        return new MockTemplatesDto(
            Templates: templatesDto
        );
    }
}