using Domain.Interfaces;
using Domain.Models;

namespace Application.MockEngine.Rules;

/// <summary>
/// Rule for server that will use pre-defined reply for every incoming request
/// </summary>
public sealed class SimpleRequestReplyEngineRule : MockEngineRuleBase
{
    private readonly MockTemplate _template;
    public SimpleRequestReplyEngineRule(INatsGate natsGate, MockTemplate template) : base(natsGate)
    {
        _template = template;
        SubscriptionId = natsGate.Subscribe(template.Topic, Handler);
    }

    private ResponseMessageData Handler(IncomingMessageData messageData) =>
        new (_template.AnswerTemplate, messageData.TopicToReply);
}