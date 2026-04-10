using Solder.Core.Interfaces.LauncherSettings;

namespace Solder.Infrastructure.Persistence.Settings;

public class SettingsService : ISettingsService
{
    public string GetApplicationPath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Solder");
    }

    public string GetInstancePath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Solder",
            "Instances");
    }

    public string GetGamePath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Solder", "Instances",
            "GameFolder");
    }

    public string SetGamePath()
    {
        throw new NotImplementedException();
    }

    public string GetTempFilePath()
    {
        throw new NotImplementedException();
    }

    public string SetTempFilePath()
    {
        throw new NotImplementedException();
    }
}