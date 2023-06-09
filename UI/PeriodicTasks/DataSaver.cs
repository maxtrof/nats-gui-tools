using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Threading;
using Domain.Interfaces;

namespace UI.PeriodicTasks;

/// <summary>
/// Periodic task to save data
/// Triggers OnDataSaved Event when data is saved and
/// changes SavedAtMessage
/// </summary>
internal sealed class DataSaver : INotifyPropertyChanged
{
    private const int SavePeriodInSeconds = 10;
    
    private readonly IDataStorage _dataStorage;
    private readonly DispatcherTimer _disTimer = new ();

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;
    public EventHandler<DateTime> OnDataSaved = (s , e) => { };

    /// <summary>
    /// Formatted text message to show
    /// </summary>
    public string SavedAtMessage { get; set; } = "Loaded";

    private DateTime _nextCycle = GetNextTick();

    public DataSaver(IDataStorage dataStorage)
    {
        _dataStorage = dataStorage ?? throw new ArgumentNullException(nameof(dataStorage));
        _disTimer.Interval = TimeSpan.FromSeconds(SavePeriodInSeconds);
        _disTimer.Tick += DispatcherTimer_Tick;
        _disTimer.Start();
    }

    private void DispatcherTimer_Tick(object? sender, System.EventArgs e)
    {
        Task.Run(_dataStorage.SaveDataIfNeeded);
        var now = DateTime.Now;
        if (now >= _nextCycle)
        {
            _nextCycle = GetNextTick();
            OnDataSaved(this, now);
        }
        SavedAtMessage = "Auto-saved at " + now.ToString("HH:mm:ss");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SavedAtMessage)));
    }
    
    private static DateTime GetNextTick() => DateTime.Now.AddSeconds(SavePeriodInSeconds);
}