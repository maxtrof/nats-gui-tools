namespace UI.MessagesBus;

public static class BusEvents
{
    // Requests
    public const string RequestSelected = "RequestSelected";
    public const string RequestUpdated = "RequestUpdated";
    public const string RequestDeleted = "RequestDeleted";
    // Listeners
    public const string ListenerSelected = nameof(ListenerSelected);
    public const string ListenerUpdated = nameof(ListenerUpdated);
    public const string ListenerDeleted = nameof(ListenerDeleted);
    // Error
    public const string ErrorThrown = nameof(ErrorThrown);
}