using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Solder.Core.Interfaces.Instances;
using Solder.Core.Interfaces.ModrinthAPI.Project;

namespace Solder.ViewModels;

public partial class BrowseViewModel : ViewModelBase
{
    private readonly IInstanceService _instanceService;
    private readonly IModrinthProjectService _modrinthProjectService;

    [ObservableProperty] private string _searchText;

    public BrowseViewModel(IInstanceService instanceService, IModrinthProjectService modrinthProjectService)
    {
        _instanceService = instanceService;
        _modrinthProjectService = modrinthProjectService;
    }

    public BrowseViewModel()
    {
        // Parameterless constructor for design-time support
        _instanceService = null!;
        _modrinthProjectService = null!;

        Instances.Add(new InstanceTemplateViewModel(_instanceService)
            { Name = "Gregtech", Version = "1.0.0", InstanceId = Guid.Empty, IsAddInstanceCard = false });
        Instances.Add(new InstanceTemplateViewModel(_instanceService)
            { Name = "SevTech", Version = "1.0.0", InstanceId = Guid.Empty, IsAddInstanceCard = false });
        Instances.Add(new InstanceTemplateViewModel(_instanceService) { IsAddInstanceCard = true });
    }

    public ObservableCollection<InstanceTemplateViewModel> Instances { get; } = new();

    async partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || _modrinthProjectService == null)
        {
            Instances.Clear();
            return;
        }

        try
        {
            var response = await _modrinthProjectService.SearchProjectByName(value, builder =>
            {
                builder.Take(40);
                builder.AddFacets([["project_type:modpack"]]);
            });
            if (response?.Hits != null)
            {
                Instances.Clear();
                foreach (var hit in response.Hits)
                    Instances.Add(new InstanceTemplateViewModel(_instanceService)
                    {
                        Name = hit.Title,
                        Version = hit.LatestVersion,
                        Description = hit.Description,
                        IsAddInstanceCard = false
                    });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Search error: {ex.Message}");
        }
    }
}