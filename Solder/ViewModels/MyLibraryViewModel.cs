using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Solder.Core.DTOs.Instance;
using Solder.Core.Interfaces.InstanceRepository;
using Solder.Core.Interfaces.Instances;
using Solder.Signals;

namespace Solder.ViewModels;

public partial class MyLibraryViewModel : ObservableObject
{
    private readonly IInstanceRepository _instanceRepository;
    private readonly IInstanceService _instanceService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty] private object? _currentModal;
    [ObservableProperty] private int _currentProgress;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isCreatorWindowOpen;
    [ObservableProperty] private bool _isSettingsWindowOpen;

    public MyLibraryViewModel()
    {
        _instanceRepository = null!;
        _instanceService = null!;
        _serviceProvider = null!;

        Instances.Add(new InstanceTemplateViewModel(_instanceService)
            { Name = "Gregtech", Version = "1.0.0", InstanceId = Guid.Empty, IsAddInstanceCard = false });
        Instances.Add(new InstanceTemplateViewModel(_instanceService)
            { Name = "SevTech", Version = "1.0.0", InstanceId = Guid.Empty, IsAddInstanceCard = false });
        Instances.Add(new InstanceTemplateViewModel(_instanceService) { IsAddInstanceCard = true });
    }

    public MyLibraryViewModel(IServiceProvider serviceProvider, IInstanceRepository instanceRepository,
        IInstanceService instanceService)
    {
        _serviceProvider = serviceProvider;
        _instanceRepository = instanceRepository;
        _instanceService = instanceService;
        _instanceRepository.LoadInstanceRepositoryAsync();
        _ = LoadInstancesAsync();

        WeakReferenceMessenger.Default.Register<OpenInstanceCreator>(this,
            (recipient, message) => { _ = OpenInstanceCreator(); });

        WeakReferenceMessenger.Default.Register<OpenInstanceSettings>(this, (recipient, message) =>
        {
            Console.WriteLine(message.InstanceUuid.ToString());
            _ = OpenSettings(message.InstanceUuid);
        });

        WeakReferenceMessenger.Default.Register<CloseCurrentWindow>(this, (recipient, message) =>
        {
            _ = ReLoadInstancesAsync();
            _ = CloseModal();
        });
    }

    public ObservableCollection<InstanceTemplateViewModel> Instances { get; } = new();
    public string Greeting { get; } = "Welcome to Avalonia!";

    [RelayCommand]
    private async Task OpenSettings(Guid instanceUuid)
    {
        if (_serviceProvider == null) return;
        var temp = _serviceProvider.GetService(typeof(InstanceSettingsViewModel)) as InstanceSettingsViewModel;
        if (temp == null) return;
        temp.InstanceUuid = instanceUuid;
        CurrentModal = temp;
    }

    [RelayCommand]
    private async Task OpenInstanceCreator()
    {
        if (_serviceProvider == null) return;
        var temp = _serviceProvider.GetService(typeof(InstanceCreatorViewModel)) as InstanceCreatorViewModel;
        if (temp == null) return;
        CurrentModal = temp;
    }

    [RelayCommand]
    public async Task CloseModal()
    {
        CurrentModal = null;
    }

    private async Task LoadInstancesAsync()
    {
        if (_instanceRepository == null) return;

        try
        {
            await _instanceRepository.LoadInstanceRepositoryAsync();
            var data = _instanceRepository.GetInstances();

            var newItems = data.Instances.Select(instance => new InstanceTemplateViewModel(_instanceService)
            {
                Name = instance.Name,
                Version = instance.Version,
                Description = instance.Description,
                InstanceId = instance.InstanceId,
                IsAddInstanceCard = false
            }).ToList();

            newItems.Add(new InstanceTemplateViewModel(_instanceService) { IsAddInstanceCard = true });

            Dispatcher.UIThread.Post(() =>
            {
                Instances.Clear();
                foreach (var item in newItems) Instances.Add(item);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading instances: {ex.Message}");
        }
    }

    private async Task CloseInstanceCreator()
    {
        IsCreatorWindowOpen = false;
    }

    private async Task ReLoadInstancesAsync()
    {
        if (_instanceRepository == null) return;
        try
        {
            await _instanceRepository.LoadInstanceRepositoryAsync();
            var data = _instanceRepository.GetInstances().Instances;
            var counter = 0;

            foreach (var newInstanceCandidate in data)
                try
                {
                    Instances.First(x => x.InstanceId == newInstanceCandidate.InstanceId);
                }
                catch (Exception)
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        try
                        {
                            Instances.Remove(Instances.First(x => x.IsAddInstanceCard));
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("No AddInstanceCard");
                        }

                        Instances.Add(new InstanceTemplateViewModel(_instanceService)
                        {
                            Name = newInstanceCandidate.Name,
                            Version = newInstanceCandidate.Version,
                            Description = newInstanceCandidate.Description,
                            InstanceId = newInstanceCandidate.InstanceId,
                            IsAddInstanceCard = false
                        });
                        Instances.Add(new InstanceTemplateViewModel(_instanceService)
                        {
                            IsAddInstanceCard = true
                        });
                    });
                    break;
                }

            Console.WriteLine($"Loaded {counter} instances");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading instances: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task RunInstanceAsync()
    {
        if (_instanceService == null) return;
        await _instanceService.RunInstanceAsync(new Guid("d0a22413-0926-4b5f-90eb-f3f9afb1121c"));
    }

    [RelayCommand]
    private async Task CreateInstanceAsync()
    {
        IsBusy = true;

        try
        {
            await Task.Run(async () =>
            {
                if (_instanceService == null) return;
                for (var i = 0; i <= 100; i++)
                {
                    await Task.Delay(5);
                    Dispatcher.UIThread.Post(() => CurrentProgress = i);
                }

                await _instanceService.CreateInstance(new InstanceCreationData
                (
                    "LatestInstance",
                    "1.21.11",
                    ""
                ));
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            IsBusy = false;
        }
    }
}