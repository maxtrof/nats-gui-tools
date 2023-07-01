using Application.RequestProcessing.PipelineBlocks;
using Domain.Models;

namespace Application.RequestProcessing;

/// <summary>
/// Knows how to construct and run request pipeline (async version)
/// </summary>
public sealed class Pipeline
{
    private readonly Queue<PipelineBlockBase> _pipelineBlocks = new();

    /// <summary>
    /// Adds block to a pipeline. Ordering is important.
    /// </summary>
    /// <param name="block">Pipeline block</param>
    public void AddBlock(PipelineBlockBase block)
    {
        _pipelineBlocks.Enqueue(block);
    }

    /// <summary>
    /// Runs pipeline in added order
    /// </summary>
    public async Task Run(NatsRequest request)
    {
        while (_pipelineBlocks.Count > 0)
        {
            var block = _pipelineBlocks.Dequeue();
            await block.Process(request);
        }
    }
}