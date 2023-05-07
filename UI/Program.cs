using Avalonia;
using Avalonia.ReactiveUI;
using System;
using Application;
using Application.MockEngine;
using Application.RequestProcessing;
using Application.TopicListener;
using Autofac;
using Domain.Interfaces;
using Infrastructure.Nats;
using Infrastructure.Repositories.AppDataRepository;

namespace UI;

class Program
{
    /// <summary>
    /// Autofac Container
    /// </summary>
#pragma warning disable CS8618
    public static IContainer Container { get; set; }
#pragma warning restore CS8618
    
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        SetUpDi();
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();

    /// <summary>
    /// Create container and register dependencies
    /// </summary>
    private static void SetUpDi()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<FileStorageAppDataRepository>().As<IAppDataRepository>().SingleInstance();
        builder.RegisterType<DataStorage>().As<IDataStorage>().SingleInstance();
        builder.RegisterType<NatsGate>().As<INatsGate>().SingleInstance();
        builder.RegisterType<PipelineBuilder>().AsSelf();
        builder.RegisterType<RequestProcessor>().AsSelf();
        builder.RegisterType<MockEngine>().AsSelf().SingleInstance();
        builder.RegisterType<TopicListener>().AsSelf().SingleInstance();
        builder.RegisterType<ConnectionManager>().AsSelf().SingleInstance();
        Container = builder.Build();
    }
}