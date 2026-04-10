using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Solder.Signals;

namespace Solder.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private ObservableObject _currentPage;

    public MainWindowViewModel()
    {
        _currentPage = App.ServiceProvider.GetRequiredService<MyLibraryViewModel>();

        WeakReferenceMessenger.Default.Register<CloseCurrentWindow>(this, (r, m) => { ShowLibrary(); });
    }


    [RelayCommand]
    public void ShowHome()
    {
        CurrentPage = App.ServiceProvider.GetRequiredService<HomeViewModel>();
    }

    [RelayCommand]
    public void ShowLibrary()
    {
        CurrentPage = App.ServiceProvider.GetRequiredService<MyLibraryViewModel>();
    }

    [RelayCommand]
    public void ShowBrowse()
    {
        CurrentPage = App.ServiceProvider.GetRequiredService<BrowseViewModel>();
    }

    [RelayCommand]
    public void ShowDownloads()
    {
        CurrentPage = App.ServiceProvider.GetRequiredService<DownloadsViewModel>();
    }

    [RelayCommand]
    public void ShowProfile()
    {
        WeakReferenceMessenger.Default.Send(new OpenProfileWindow());
    }
}