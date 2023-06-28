namespace Domain.Models;

public record ConnectionStats(
  long MessagesSent,
  long MessagesReceived,
  long Reconnects,
  bool JetStreamAvailable,
  long SubscriptionsCount
);