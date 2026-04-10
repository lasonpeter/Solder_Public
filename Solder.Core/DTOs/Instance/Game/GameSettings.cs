namespace Solder.Core.DTOs.Instance.Game;

public class GameSettings
{
    //Max RAM(MB)
    public long Xmx { get; set; } = 4096;

    //Min RAM(MB)
    public long Xms { get; set; } = 2048;

    //JAVA path
    public string JavaPath { get; set; } = string.Empty;

    //Custom Java arguments
    public string JavaArgs { get; set; } = string.Empty;
}