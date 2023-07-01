using Application.MockEngine.Rules;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;

namespace Application.MockEngine;

/// <summary>
/// App mock engine. Set up mocks and manages them
/// </summary>
public sealed class MockEngine : IDisposable
{
    private readonly INatsGate _natsGate;
    /// <summary>
    /// Active rules 
    /// </summary>
    private readonly List<MockEngineRuleBase> _rules = new ();
    
    /// <summary>
    /// Event, firing on rule stop, passing rule Id
    /// </summary>
    public event EventHandler<Guid> OnRuleStopped = default!;
    
    public MockEngine(INatsGate natsGate)
    {
        _natsGate = natsGate ?? throw new ArgumentNullException(nameof(natsGate));
    }

    /// <summary>
    /// Activates a rule
    /// </summary>
    /// <param name="template">Mock template</param>
    /// <returns>Rule's Id</returns>
    public async Task<Guid> ActivateRule(MockTemplate template)
    {
        MockEngineRuleBase rule;

        switch (template.Type)
        {
            case MockTypes.SimpleRequestReply:
                rule = new SimpleRequestReplyEngineRule(_natsGate, template);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        await rule.Initialize();
        _rules.Add(rule);
        return rule.Id;
    }

    /// <summary>
    /// Deactivates rule
    /// </summary>
    /// <param name="id">Rule's Id</param>
    public void DeactivateRule(Guid id)
    {
        var rule = _rules.Find(x => x.Id == id);
        if (rule is null) return;
        
        DeactivateRuleInternal(rule);
    }

    /// <summary>
    /// Deactivates all rules
    /// </summary>
    public void DeactivateAllRules()
    {
        for (var index = 0; index < _rules.Count; index++)
        {
            DeactivateRuleInternal(_rules[index]);
        }
    }
    
    private void DeactivateRuleInternal(MockEngineRuleBase rule)
    {
        rule.Stop();
        _rules.Remove(rule);
        OnRuleStopped.Invoke(this, rule.Id);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var mockEngineRuleBase in _rules)
        {
            mockEngineRuleBase.Dispose();
        }
    }
}