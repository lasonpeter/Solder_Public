namespace Solder.Core.DTOs.Instance;

public class InstanceData
{
    public Guid InstanceId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Version { get; set; }
    public required string Path { get; set; }
}