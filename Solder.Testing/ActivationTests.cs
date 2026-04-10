using Microsoft.Extensions.DependencyInjection;
using Solder.Core.Interfaces.InstanceRepository;
using Solder.Core.Interfaces.Instances;
using Solder.Core.Interfaces.LauncherSettings;
using Solder.ViewModels;

namespace Solder.Testing;

public class ActivationTests
{
    [Fact]
    public void AllServicesCanBeResolved_ExceptViews()
    {
        var collection = new ServiceCollection();
        collection.AddCommonServices();
        var provider = collection.BuildServiceProvider();

        // Check each service except MainWindow which requires Avalonia platform
        Assert.NotNull(provider.GetService<ISettingsService>());
        Assert.NotNull(provider.GetService<IGameSettingsService>());
        Assert.NotNull(provider.GetService<IInstanceService>());
        Assert.NotNull(provider.GetService<IInstanceRepository>());
        Assert.NotNull(provider.GetService<MainWindowViewModel>());
    }

    [Fact]
    public void MainWindowViewModel_ResolvesWithRequiredDependencies()
    {
        var collection = new ServiceCollection();
        collection.AddCommonServices();
        var provider = collection.BuildServiceProvider();

        var vm = provider.GetRequiredService<MainWindowViewModel>();
        Assert.NotNull(vm);
    }
}