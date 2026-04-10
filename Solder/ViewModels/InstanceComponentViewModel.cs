using CommunityToolkit.Mvvm.ComponentModel;

namespace Solder.ViewModels;

public partial class InstanceComponentViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _name = "Unnamed Instance";

    public InstanceComponentViewModel()
    {
        
    }
}