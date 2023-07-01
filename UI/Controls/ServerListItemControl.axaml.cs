using System;
using System.Reactive.Concurrency;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using UI.EventArgs;
using UI.ViewModels;

namespace UI.Controls;

internal partial class ServerListItemControl : ReactiveUserControl<ServerListItemViewModel>
{
    public static readonly RoutedEvent<RoutedEventArgs> UpdateServersStateEvent =
        RoutedEvent.Register<ServerListItemControl, RoutedEventArgs>(nameof(UpdateServersState), RoutingStrategies.Bubble);
   
    public event EventHandler<RoutedEventArgs> UpdateServersState
    { 
        add => AddHandler(UpdateServersStateEvent, value);
        remove => RemoveHandler(UpdateServersStateEvent, value);
    }
    
    public static readonly RoutedEvent<UpdateServerRoutedEventArgs> EditServerEvent =
        RoutedEvent.Register<ServerListItemControl, UpdateServerRoutedEventArgs>(nameof(EditServerEvent), RoutingStrategies.Bubble);
   
    public event EventHandler<UpdateServerRoutedEventArgs> EditServer
    { 
        add => AddHandler(EditServerEvent, value);
        remove => RemoveHandler(EditServerEvent, value);
    }
    
    public static readonly RoutedEvent<DeleteServerRoutedEventArgs> DeleteServerEvent =
        RoutedEvent.Register<ServerListItemControl, DeleteServerRoutedEventArgs>(nameof(DeleteServerEvent), RoutingStrategies.Bubble);
   
    public event EventHandler<DeleteServerRoutedEventArgs> DeleteServer
    { 
        add => AddHandler(DeleteServerEvent, value);
        remove => RemoveHandler(DeleteServerEvent, value);
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
        var shouldUpdateServers = await ViewModel!.ConnectOrDisconnectToAServer();
        if (shouldUpdateServers)
            RaiseEvent(new RoutedEventArgs { RoutedEvent = UpdateServersStateEvent });
    }

    private void ConnectDisconnectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RxApp.MainThreadScheduler.Schedule(ConnectOrDisconnectToAServer);
    }

    private void EditButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new UpdateServerRoutedEventArgs { RoutedEvent = EditServerEvent, Id = ViewModel!.Id});
    }

    private void DeleteButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new DeleteServerRoutedEventArgs { RoutedEvent = DeleteServerEvent, Id = ViewModel!.Id});
    }
}