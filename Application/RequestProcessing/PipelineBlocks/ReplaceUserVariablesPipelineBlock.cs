using System.Text;
using Domain.Models;

namespace Application.RequestProcessing.PipelineBlocks;

/// <summary>
/// Pipeline block that replaces variables in request body with user-defined values.
/// </summary>
internal sealed class ReplaceUserVariablesPipelineBlock : PipelineBlockBase
{
    private readonly Dictionary<string, string> _dictionary;

    public ReplaceUserVariablesPipelineBlock(Dictionary<string, string> userDictionary)
    {
        _dictionary = userDictionary;
    }
    
    /// <inheritdoc />
    public override Task Process(NatsRequest request)
    {
        var sb = new StringBuilder(request.Body);
        foreach (var (key, value) in _dictionary)
        {
            sb.Replace($"${key}", value);
        }

        request.Body = sb.ToString();
        
        return Task.CompletedTask;
    }
}