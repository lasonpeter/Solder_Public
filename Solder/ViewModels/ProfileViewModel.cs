using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Solder.Core.DTOs.Api.Solder;
using Solder.Core.Interfaces.Api.Solder;
using LoginRequest = Solder.Shared.DTOs.Solder.Auth.LoginRequest;
using RegisterRequest = Solder.Shared.DTOs.Solder.Auth.RegisterRequest;

namespace Solder.ViewModels;

public partial class ProfileViewModel : ViewModelBase
{
    private readonly ISolderAuthService _solderAuthService;
    private readonly ISolderProjectApiService _solderProjectApiService;

    [ObservableProperty] private string _email = string.Empty;

    [ObservableProperty] private string? _errorMessage;

    [ObservableProperty] private bool _isLoggedIn;

    [ObservableProperty] private string? _newProjectDescription;

    [ObservableProperty] private string _newProjectName = string.Empty;

    [ObservableProperty] private string _password = string.Empty;

    [ObservableProperty] private ObservableCollection<ProjectResponse> _projects = new();

    [ObservableProperty] private string? _userInfo;

    [ObservableProperty] private string _userName = string.Empty;

    public ProfileViewModel(ISolderAuthService solderAuthService, ISolderProjectApiService solderProjectApiService)
    {
        _solderAuthService = solderAuthService;
        _solderProjectApiService = solderProjectApiService;
        IsLoggedIn = _solderAuthService.IsAuthenticatedAsync();
        if (IsLoggedIn)
            _ = LoadProfileCached();
    }

    [RelayCommand]
    public async Task Login()
    {
        ErrorMessage = null;
        try
        {
            var response = await _solderAuthService.LoginAsync(new LoginRequest(_email, _password));
            if (response != null)
            {
                IsLoggedIn = true;
                await LoadProfile();
            }
            else
            {
                ErrorMessage = "Login failed. Please check your credentials or if your account is confirmed.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task Register()
    {
        ErrorMessage = null;
        var success = await _solderAuthService.RegisterAsync(new RegisterRequest(_email, _userName, _password));
        if (success)
            ErrorMessage = "Registration successful. Please login.";
        else
            ErrorMessage = "Registration failed.";
    }

    [RelayCommand]
    public void Logout()
    {
        _solderAuthService.LogoutAsync();
        IsLoggedIn = false;
        UserInfo = null;
        Projects.Clear();
    }

    public async Task LoadProfile()
    {
        var info = _solderAuthService.GetUserInfo();
        if (info != null) UserInfo = $"Logged in as: {info.UserName}";
        var projects = await _solderProjectApiService.GetProjectsAsync();
        Projects.Clear();
        foreach (var p in projects) Projects.Add(p);
    }

    public async Task LoadProfileCached()
    {
        var info = _solderAuthService.GetUserInfo();
        if (info != null) UserInfo = $"Logged in as: {info.UserName}";
        var projects = await _solderProjectApiService.GetProjectsAsync();
        Projects.Clear();
        foreach (var p in projects) Projects.Add(p);
    }

    /*[RelayCommand]
    public async Task CreateProject()
    {
        if (string.IsNullOrWhiteSpace(NewProjectName)) return;

        var result = await _apiService.CreateProjectAsync(new CreateProjectRequest
        {
            ProjectName = NewProjectName,
            ProjectDescription = NewProjectDescription
        });

        if (result != null)
        {
            Projects.Add(result);
            NewProjectName = string.Empty;
            NewProjectDescription = null;
        }
    }

    [RelayCommand]
    public async Task DeleteProject(ProjectResponse project)
    {
        if (project == null) return;
        var success = await _apiService.DeleteProjectAsync(project.ProjectId);
        if (success)
        {
            Projects.Remove(project);
        }
    }*/
}