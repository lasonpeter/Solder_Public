using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Solder.Core.DTOs.Instance.Game;
using Solder.Core.Interfaces.Instances;
using Solder.Signals;

namespace Solder.ViewModels;

public partial class InstanceSettingsViewModel : ObservableObject
{
    private readonly IGameSettingsService _gameSettingsService;
    private GameSettings _gameSettings;

    [ObservableProperty] private Guid _instanceUuid;
    [ObservableProperty] private string _jvmArgs = "-XX:+UseG1GC";
    [ObservableProperty] private long _maxMem = 4096;
    [ObservableProperty] private long _minMem = 512;

    [ObservableProperty] private ModItemViewModel? _selectedMod;
    [ObservableProperty] private ResourcePackViewModel? _selectedResourcePack;

    public InstanceSettingsViewModel(IGameSettingsService gameSettingsService)
    {
        _gameSettingsService = gameSettingsService;
    }

    public ObservableCollection<ModItemViewModel> ModList { get; } = new();
    public ObservableCollection<ResourcePackViewModel> ResourcePacks { get; } = new();

    partial void OnInstanceUuidChanged(Guid value)
    {
        LoadData();
    }

    private void LoadData()
    {
        if (InstanceUuid == Guid.Empty) return;

        _gameSettings = _gameSettingsService.LoadGameSettings(InstanceUuid);
        MinMem = _gameSettings.Xms;
        MaxMem = _gameSettings.Xmx;
        JvmArgs = _gameSettings.JavaArgs;

        ModList.Add(new ModItemViewModel { Name = "Sodium", Version = "0.5.8", IsEnabled = true });
        ModList.Add(new ModItemViewModel { Name = "Iris", Version = "1.7.0", IsEnabled = true });

        ResourcePacks.Add(new ResourcePackViewModel { Name = "Faithful 32x", Description = "The classic look." });
    }

    [RelayCommand]
    public async Task CloseSettingsAsync()
    {
        WeakReferenceMessenger.Default.Send(new CloseCurrentWindow());
    }

    [RelayCommand]
    public async Task SaveSettingsAsync()
    {
        _gameSettings.Xms = MinMem;
        _gameSettings.Xmx = MaxMem;
        _gameSettings.JavaArgs = JvmArgs;
        await _gameSettingsService.SaveGameSettings(InstanceUuid, _gameSettings);
        _ = CloseSettingsAsync();
    }

    partial void OnMinMemChanged(long value)
    {
        if (value > MaxMem) MaxMem = value;
    }

    partial void OnMaxMemChanged(long value)
    {
        if (value < MinMem) MinMem = value;
    }
}

// Sub-ViewModels using the same Toolkit style
public partial class ModItemViewModel : ObservableObject
{
    [ObservableProperty] private bool _isEnabled;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _version = string.Empty;
}

public partial class ResourcePackViewModel : ObservableObject
{
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private string _name = string.Empty;
}