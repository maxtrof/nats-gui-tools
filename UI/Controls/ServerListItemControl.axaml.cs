using System;
using System.Reactive.Concurrency;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using UI.ViewModels;

namespace UI.Controls;

internal partial class ServerListItemControl : UserControl
{
    public static readonly RoutedEvent<RoutedEventArgs> UpdateServersStateEvent =
        RoutedEvent.Register<ServerListItemControl, RoutedEventArgs>(nameof(UpdateServersState), RoutingStrategies.Bubble);
   
    public event EventHandler<RoutedEventArgs> UpdateServersState
    { 
        add => AddHandler(UpdateServersStateEvent, value);
        remove => RemoveHandler(UpdateServersStateEvent, value);
    }
   
   public ServerListItemControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

   
    private async void ConnectOrDisconnectToAServer()
    {
        var shouldUpdateServers = await ((ServerListItemViewModel)DataContext!).ConnectOrDisconnectToAServer();
        if (shouldUpdateServers)
            RaiseEvent(new RoutedEventArgs { RoutedEvent = UpdateServersStateEvent });
    }

    private void ConnectDisconnectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RxApp.MainThreadScheduler.Schedule(ConnectOrDisconnectToAServer);
    }
}