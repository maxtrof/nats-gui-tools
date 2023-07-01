using Domain.Interfaces;

namespace Application.MockEngine.Rules;

/// <summary>
/// Base for Mock engine rules. 
/// </summary>
public abstract class MockEngineRuleBase : IDisposable
{
    protected long SubscriptionId { init; get; }
    private readonly INatsGate _natsGate;
    
    protected MockEngineRuleBase(INatsGate natsGate)
    {
        _natsGate = natsGate;
    }

    /// <summary>
    /// Rule Id
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Initializes rule
    /// </summary>
    public Task Initialize()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops rule
    /// </summary>
    public void Stop()
    {
        _natsGate.Unsubscribe(SubscriptionId);
    }

    /// <inheritdoc />
    public void Dispose() => Stop();
}