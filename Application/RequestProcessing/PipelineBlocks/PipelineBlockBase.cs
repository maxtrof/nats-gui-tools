using Domain.Models;

namespace Application.RequestProcessing.PipelineBlocks;

/// <summary>
/// Base class for pipeline blocks
/// </summary>
public abstract class PipelineBlockBase
{
    /// <summary>
    /// Transforms and processes request
    /// </summary>
    /// <param name="request">Request</param>
    public abstract Task Process(NatsRequest request);
}