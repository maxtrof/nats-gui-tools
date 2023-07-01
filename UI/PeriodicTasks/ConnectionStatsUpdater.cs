using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Application;
using Avalonia.Threading;
using Domain.Interfaces;

namespace UI.PeriodicTasks;

internal sealed class ConnectionStatsUpdater : INotifyPropertyChanged
{
    private const string DefaultMessage = "Connect to a server or create a new one.";
    private const int UpdatePeriodInSeconds = 1;
    
    private readonly ConnectionManager _connectionManager;
    private readonly DispatcherTimer _disTimer = new ();

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;
    public EventHandler<DateTime> OnStatsUpdated = (s , e) => { };

    /// <summary>
    /// Formatted text message to show
    /// </summary>
    public string StatsTextMessage { get; set; } = DefaultMessage;

    private DateTime _nextCycle = GetNextTick();

    public ConnectionStatsUpdater(ConnectionManager connectionManager)
    {
        _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
        _disTimer.Interval = TimeSpan.FromSeconds(UpdatePeriodInSeconds);
        _disTimer.Tick += DispatcherTimer_Tick;
        _disTimer.Start();
    }

    private void DispatcherTimer_Tick(object? sender, System.EventArgs e)
    {
        if (_connectionManager.IsConnected)
        {
            var stats = _connectionManager.GetStats;
            StatsTextMessage = $@"Connection data: 

    Messages Sent: {stats.MessagesSent}
    Messages Received: {stats.MessagesReceived}
    Reconnects: {stats.Reconnects}
    Subscription count: {stats.SubscriptionsCount}
    
    JetStream available: {(stats.JetStreamAvailable ? "Yes" : "No")}
            ";
        }
        else
        {
            StatsTextMessage = DefaultMessage;
        }
        var now = DateTime.Now;
        if (now >= _nextCycle)
        {
            _nextCycle = GetNextTick();
            OnStatsUpdated(this, now);
        }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatsTextMessage)));
    }
    
    private static DateTime GetNextTick() => DateTime.Now.AddSeconds(UpdatePeriodInSeconds);
}