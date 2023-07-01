namespace UI.MessagesBus;

internal static class BusEvents
{
    // Requests
    public const string RequestSelected = nameof(RequestSelected);
    public const string RequestUpdated = nameof(RequestUpdated);
    public const string RequestDeleted = nameof(RequestDeleted);
    // Listeners
    public const string ListenerSelected = nameof(ListenerSelected);
    public const string ListenerUpdated = nameof(ListenerUpdated);
    public const string ListenerDeleted = nameof(ListenerDeleted);
    // Mocks
    public const string MockSelected = nameof(MockSelected);
    public const string MockUpdated = nameof(MockUpdated);
    public const string MockDeleted = nameof(MockDeleted);
    // Error
    public const string ErrorThrown = nameof(ErrorThrown);
    // Autocompletion
    public const string AutocompleteAdded = nameof(AutocompleteAdded);
}