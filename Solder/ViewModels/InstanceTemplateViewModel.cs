using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Solder.Core.Interfaces.Instances;
using Solder.Signals;

namespace Solder.ViewModels;

public partial class InstanceTemplateViewModel : ObservableObject
{
    /*public bool IsAddInstanceCard
    {
        get => _isAddInstanceCard;
        set => SetProperty(ref _isAddInstanceCard, value);
    }
    */


    private readonly IInstanceService _instanceService;
    [ObservableProperty] private int _currentProgress;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private Guid _instanceId;
    [ObservableProperty] private bool _isAddInstanceCard;
    [ObservableProperty] private bool _isBeingCreated;
    [ObservableProperty] private bool _isRunning;
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _version = string.Empty;

    public InstanceTemplateViewModel(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }


    [RelayCommand]
    public async Task LaunchInstanceAsync()
    {
        IsRunning = true;
        try
        {
            Debug.WriteLine($"Starting instance: {Name} (ID: {InstanceId})");
            await _instanceService.RunInstanceAsync(InstanceId);
        }
        finally
        {
            IsRunning = false;
        }
    }

    [RelayCommand]
    public async Task CreateInstanceAsync()
    {
        WeakReferenceMessenger.Default.Send<OpenInstanceCreator>(new OpenInstanceCreator());
    }

    [RelayCommand]
    public async Task OpenInstanceSettingsAsync()
    {
        Console.WriteLine(InstanceId.ToString());
        WeakReferenceMessenger.Default.Send<OpenInstanceSettings>(new OpenInstanceSettings
        {
            InstanceUuid = InstanceId
        });
    }

    /*[RelayCommand]
    public async Task OpenInstanceAsync()
    {
        WeakReferenceMessenger.Default.Send<OpenInstanceSettings>(new OpenInstanceSettings());
    }*/
}