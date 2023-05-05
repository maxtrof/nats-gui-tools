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
        
        rule.Stop();
        _rules.Remove(rule);
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