using Domain.Interfaces;
using Domain.Models;

namespace Application.RequestProcessing;

/// <summary>
/// Processes request and request-replies
/// </summary>
public sealed class RequestProcessor
{
    private readonly Pipeline _pipeline;
    private readonly INatsGate _natsGate;

    public RequestProcessor(PipelineBuilder pipelineBuilder, INatsGate natsGate)
    {
        if (pipelineBuilder is null) throw new ArgumentNullException(nameof(pipelineBuilder));
        _natsGate = natsGate ?? throw new ArgumentNullException(nameof(natsGate));
        // Lets build a pipeline
        pipelineBuilder.AddReplaceUserVariablesBlock();
        _pipeline = pipelineBuilder.Build();
    }

    /// <summary>
    /// Sends <see cref="NatsRequest"/>
    /// </summary>
    /// <param name="template">Request template</param>
    public async Task SendRequest(RequestTemplate template)
    {
        var req = BuildRequest(template);
        await _pipeline.Run(req);
        _natsGate.SendRequest(req);
    }

    /// <summary>
    /// Send <see cref="NatsRequest"/> and gets deserialized reply
    /// </summary>
    /// <param name="template">Request template</param>
    /// <typeparam name="T">Type to deserialize answer</typeparam>
    public async Task<T> SendRequestReply<T>(RequestTemplate template)
    {
        var req = BuildRequest(template);
        await _pipeline.Run(req);
        return await _natsGate.SendRequestReply<T>(req);
    }
    
    /// <summary>
    /// Send <see cref="NatsRequest"/> and gets string reply
    /// </summary>
    /// <param name="template">Request template</param>
    public async Task<string> SendRequestReply(RequestTemplate template)
    {
        var req = BuildRequest(template);
        await _pipeline.Run(req);
        return await _natsGate.SendRequestReply(req);
    }
    

    /// <summary>
    /// Builds <see cref="NatsRequest"/> based on <see cref="RequestTemplate"/>
    /// </summary>
    /// <param name="template">Request template</param>
    private NatsRequest BuildRequest(RequestTemplate template)
    {
        var req = new NatsRequest();
        req.Body = template.Body;
        req.Topic = template.Topic;
        return req;
    }
}