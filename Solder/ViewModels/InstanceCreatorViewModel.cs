using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Solder.Core.DTOs.Instance;
using Solder.Core.Interfaces.Instances;
using Solder.Signals;

namespace Solder.ViewModels;

public partial class InstanceCreatorViewModel : ObservableObject
{
    private readonly IInstanceService _instanceService;
    [ObservableProperty] private string _description = string.Empty;

    [ObservableProperty] private string _instanceName = string.Empty;
    [ObservableProperty] private string? _selectedGameVersion;
    [ObservableProperty] private string? _selectedModloader;


    public InstanceCreatorViewModel(IInstanceService instanceService)
    {
        _instanceService = instanceService;
    }


    // These would be populated from your game data service
    public ObservableCollection<string> GameVersions { get; } = new() { "1.20.1", "1.19.2", "1.18.2", "1.7.10" };
    public ObservableCollection<string> Modloaders { get; } = new() { "Vanilla", "Forge", "Fabric" };

    [RelayCommand]
    public async Task Create()
    {
        // Trigger your logic to create the folders/files
        Console.WriteLine($"Creating {InstanceName} on {SelectedGameVersion} with {SelectedModloader}");

        void InstanceServiceOnOnInstanceCreationProgressChanged(Guid instanceId, string instanceName, long progressed,
            long total)
        {
            Console.WriteLine((int)Math.Clamp(progressed / (float)total * 100, 0, 100));
        }

        ;
        _instanceService.OnInstanceCreationProgressChanged += InstanceServiceOnOnInstanceCreationProgressChanged;

        await _instanceService.CreateInstance(new InstanceCreationData
        (
            InstanceName,
            SelectedGameVersion ?? "Unknown",
            Description
        ));

        WeakReferenceMessenger.Default.Send(new CloseCurrentWindow());
    }

    [RelayCommand]
    public async Task Close()
    {
        WeakReferenceMessenger.Default.Send(new CloseCurrentWindow());
    }
}

/*
 * Console.WriteLine("Creating instance");
        IsBeingCreated = true;
        void InstanceServiceOnOnInstanceCreationProgressChanged(Guid instanceId, string instanceName, long progressed, long total)
        {
            Dispatcher.UIThread.Post(() => CurrentProgress = (int)Math.Clamp(progressed / (float)total * 100, 0, 100));
        };

        try
        {
            await Task.Run(async () =>
            {
                _instanceService.OnInstanceCreationProgressChanged += InstanceServiceOnOnInstanceCreationProgressChanged;

                await _instanceService.CreateInstance(new InstanceCreationData()
                {
                    Name = "LatestInstance",
                    Version = "1.21.11"
                });
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _instanceService.OnInstanceCreationProgressChanged -= InstanceServiceOnOnInstanceCreationProgressChanged;
            IsBeingCreated = false;
        }
 */