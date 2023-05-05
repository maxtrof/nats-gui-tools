using Application.RequestProcessing.PipelineBlocks;
using Domain.Interfaces;

namespace Application.RequestProcessing;

/// <summary>
/// Builds pipelines
/// </summary>
public sealed class PipelineBuilder
{
    private readonly IDataStorage _dataStorage;
    
    private readonly Pipeline _pipeline = new ();

    public PipelineBuilder(IDataStorage dataStorage)
    {
        _dataStorage = dataStorage;
    }

    /// <summary>
    /// Adds <see cref="ReplaceUserVariablesPipelineBlock"/> to the pipeline
    /// </summary>
    public void AddReplaceUserVariablesBlock() => 
        _pipeline.AddBlock(new ReplaceUserVariablesPipelineBlock(_dataStorage.AppSettings.UserDictionary));

    /// <summary>
    /// Return final pipeline
    /// </summary>
    public Pipeline Build() => _pipeline;
}