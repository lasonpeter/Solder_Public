using Solder.Core.DTOs.Instance;

namespace Solder.Core.Interfaces.Instances;

/// <summary>
///     Provides services for creating, running, and managing game instances.
/// </summary>
public interface IInstanceService
{
    /// <summary>
    ///     Delegate for reporting progress during instance creation.
    /// </summary>
    /// <param name="instanceId">The ID of the instance being created.</param>
    /// <param name="instanceName">The name of the instance.</param>
    /// <param name="Progressed">The amount of progress made.</param>
    /// <param name="Total">The total progress expected.</param>
    delegate void InstanceCreationProgressChanged(Guid instanceId, string instanceName, long Progressed, long Total);

    /// <summary>
    ///     Fired when the progress of instance creation changes.
    /// </summary>
    event InstanceCreationProgressChanged? OnInstanceCreationProgressChanged;

    /// <summary>
    ///     Creates a new instance based on the provided configuration.
    /// </summary>
    /// <param name="instanceCreationData">The configuration data for the new instance.</param>
    /// <returns>A task that represents the asynchronous creation operation.</returns>
    Task CreateInstance(InstanceCreationData instanceCreationData);

    /// <summary>
    ///     Starts and runs the specified instance.
    /// </summary>
    /// <param name="instanceId">The ID of the instance to run.</param>
    /// <returns>A task that represents the asynchronous run operation.</returns>
    Task RunInstanceAsync(Guid instanceId);

    /// <summary>
    ///     Deletes the specified instance and its associated files.
    /// </summary>
    /// <param name="instanceId">The ID of the instance to delete.</param>
    void DeleteInstance(Guid instanceId);
}