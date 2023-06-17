using System.Collections.ObjectModel;

namespace UI;

/// <summary>
/// Static class to keep shared observables
/// </summary>
internal static class SharedObservables
{
    public static ObservableCollection<string> Suggestions { get; set; } = new();
}