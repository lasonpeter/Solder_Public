using Solder.Core.DTOs.Instance;

namespace Solder.Core.DTOs.InstanceRepository;

public class InstanceRepositoryData
{
    public InstanceRepositoryData()
    {
        Instances = new List<InstanceData>();
    }

    public List<InstanceData> Instances { get; set; }
}