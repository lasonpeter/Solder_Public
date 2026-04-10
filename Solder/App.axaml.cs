using System;
using System.Linq;
using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Solder.Controls.UserControl;
using Solder.Core.Interfaces.Api.Solder;
using Solder.Core.Interfaces.InstanceRepository;
using Solder.Core.Interfaces.Instances;
using Solder.Core.Interfaces.LauncherSettings;
using Solder.Core.Interfaces.ModrinthAPI.Project;
using Solder.Infrastructure.Persistence.InstanceRepository;
using Solder.Infrastructure.Persistence.Instances;
using Solder.Infrastructure.Persistence.ModrinthAPI;
using Solder.Infrastructure.Persistence.Settings;
using Solder.Infrastructure.Persistence.SolderAPI;
using Solder.Shared.Statics;
using Solder.ViewModels;
using Solder.Views;

namespace Solder;

public class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        BindingPlugins.DataValidators.RemoveAt(0);

        var collection = new ServiceCollection();
        collection.AddCommonServices();

        ServiceProvider = collection.BuildServiceProvider();

        var vm = ServiceProvider.GetRequiredService<MainWindowViewModel>();
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.DataContext = vm;
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) desktop.MainWindow = mainWindow;

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove) BindingPlugins.DataValidators.Remove(plugin);
    }
}

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<HttpClient>(sp => new HttpClient { BaseAddress = new Uri(SolderUris.FallbackBaseUrl) });
        collection.AddSingleton<ISettingsService, SettingsService>();
        collection.AddSingleton<IGameSettingsService, GameSettingsService>();
        collection.AddSingleton<IInstanceService, InstanceService>();
        collection.AddSingleton<IInstanceRepository, InstanceRepository>();
        collection.AddSingleton<IModrinthProjectService, ModrinthProjectService>();
        collection.AddSingleton<ISolderApiService, SolderApiService>();
        collection.AddSingleton<ISolderAuthSessionStore, SolderAuthSessionStore>();
        collection.AddSingleton<ISolderAuthService, SolderAuthService>();
        collection.AddSingleton<ISolderProjectApiService, SolderProjectApiService>();

        collection.AddTransient<MainWindowViewModel>();
        collection.AddTransient<InstanceCreatorViewModel>();
        collection.AddTransient<InstanceSettingsViewModel>();
        collection.AddTransient<HomeViewModel>();
        collection.AddTransient<MyLibraryViewModel>();
        collection.AddTransient<BrowseViewModel>();
        collection.AddTransient<DownloadsViewModel>();
        collection.AddTransient<ProfileViewModel>();
        collection.AddTransient<InstanceTemplateViewModel>();

        collection.AddTransient<MainWindow>();
        collection.AddTransient<ProfileWindow>();
        collection.AddTransient<InstanceCreator>();
        collection.AddTransient<InstanceSettingsView>();
        collection.AddTransient<Home>();
        collection.AddTransient<MyLibrary>();
        collection.AddTransient<Browse>();
        collection.AddTransient<Downloads>();
    }
}