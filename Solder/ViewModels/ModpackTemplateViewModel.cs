using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Solder.Core.DTOs.Modrinth;

namespace Solder.ViewModels;

public partial class ModpackTemplateViewModel : ObservableObject
{
    [ObservableProperty] private string _author = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private int _downloads;
    [ObservableProperty] private string? _iconUrl;
    [ObservableProperty] private string _latestVersion = string.Empty;
    [ObservableProperty] private string _projectId = string.Empty;
    [ObservableProperty] private string _title = string.Empty;

    public ModpackTemplateViewModel()
    {
    }

    public ModpackTemplateViewModel(ModpackHit hit)
    {
        Title = hit.Title;
        Description = hit.Description;
        Author = hit.Author;
        IconUrl = hit.IconUrl;
        LatestVersion = hit.LatestVersion;
        Downloads = hit.Downloads;
        ProjectId = hit.ProjectId;
    }

    [RelayCommand]
    public async Task InstallAsync()
    {
        // TODO: Implement installation logic
        Console.WriteLine($"Installing modpack: {Title} ({ProjectId})");
    }

    [RelayCommand]
    public async Task OpenDetailsAsync()
    {
        // TODO: Implement opening details view
        Console.WriteLine($"Opening details for: {Title} ({ProjectId})");
    }
}