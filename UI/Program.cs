using Avalonia;
using Avalonia.ReactiveUI;
using System;
using Autofac;
using Domain.Interfaces;
using Infrastructure.Repositories.AppDataRepository;

namespace UI;

class Program
{
    /// <summary>
    /// Autofac Container
    /// </summary>
#pragma warning disable CS8618
    private static IContainer Container { get; set; }
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
        builder.RegisterType<FileStorageAppDataRepository>().As<IAppDataRepository>();
        Container = builder.Build();
    }
}