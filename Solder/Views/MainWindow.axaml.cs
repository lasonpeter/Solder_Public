using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Solder.Signals;
using Solder.ViewModels;

namespace Solder.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        WeakReferenceMessenger.Default.Register<OpenProfileWindow>(this, (r, m) =>
        {
            var profileWindow = App.ServiceProvider.GetRequiredService<ProfileWindow>();
            profileWindow.DataContext = App.ServiceProvider.GetRequiredService<ProfileViewModel>();
            profileWindow.ShowDialog(this);
        });
    }
}