using Solder.Core.DTOs.Instance;
using Solder.Core.DTOs.InstanceRepository;

namespace Solder.Core.Interfaces.InstanceRepository;

/// <summary>
///     Represents a repository for managing local instance data and records.
/// </summary>
public interface IInstanceRepository
{
    /// <summary>
    ///     Loads the instance repository data from the local database into memory.
    /// </summary>
    /// <returns>A task that represents the asynchronous load operation.</returns>
    public Task LoadInstanceRepositoryAsync();

    /// <summary>
    ///     Saves the current state of the instance repository from memory to the local database.
    /// </summary>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    public Task SaveInstanceRepository();

    /// <summary>
    ///     Creates a new instance record in the repository.
    /// </summary>
    /// <param name="instanceId">The unique identifier of the instance.</param>
    /// <param name="name">The name of the instance.</param>
    /// <param name="description">The description of the instance.</param>
    /// <param name="path">The local path to the instance files.</param>
    /// <param name="version">The version of the instance.</param>
    /// <returns>A task that represents the asynchronous creation operation.</returns>
    public Task CreateInstanceRecord(Guid instanceId, string name, string description, string path, string version);

    /// <summary>
    ///     Retrieves all instance records stored in the repository.
    /// </summary>
    /// <returns>An <see cref="InstanceRepositoryData" /> object containing all instances.</returns>
    public InstanceRepositoryData GetInstances();

    /// <summary>
    ///     Retrieves the data for a specific instance.
    /// </summary>
    /// <param name="instanceId">The unique identifier of the instance to retrieve.</param>
    /// <returns>An <see cref="InstanceData" /> object for the specified instance.</returns>
    public InstanceData GetInstanceData(Guid instanceId);
}