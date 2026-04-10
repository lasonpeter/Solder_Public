using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.Input;

namespace Solder.Controls.TemplatedControls;

public partial class ModpackBrowserComponentViewModel : Avalonia.Controls.UserControl
{
    public static readonly StyledProperty<string?> ModpackNameProperty =
        AvaloniaProperty.Register<ModpackBrowserComponent, string?>(nameof(ModpackName));

    public static readonly StyledProperty<string?> IconUrlProperty =
        AvaloniaProperty.Register<ModpackBrowserComponent, string?>(nameof(IconUrl));

    public string? ModpackName
    {
        get => GetValue(ModpackNameProperty);
        set => SetValue(ModpackNameProperty, value);
    }

    public string? IconUrl
    {
        get => GetValue(IconUrlProperty);
        set => SetValue(IconUrlProperty, value);
    }

    [RelayCommand]
    public async Task ShowInstallWindow()
    {
    }
}