using Microsoft.Extensions.Configuration;
using Solder.ServerInstanceManager.Infrastructure.Persistance;

namespace Solder.Testing;

public class ServerInstanceInfoServiceTests
{
    [Fact]
    public void Constructor_ShouldSetServerInstanceId_WhenEnvironmentVariableIsPresent()
    {
        // Arrange
        var expectedId = Guid.NewGuid();
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "SERVER_INSTANCE_ID", expectedId.ToString() }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act
        var service = new ServerInstanceInfoService(configuration);
        var actualId = service.GetServerGuidAsync();

        // Assert
        Assert.Equal(expectedId, actualId);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenEnvironmentVariableIsMissing()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new ServerInstanceInfoService(configuration));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenEnvironmentVariableIsInvalid()
    {
        // Arrange
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "SERVER_INSTANCE_ID", "invalid-guid" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new ServerInstanceInfoService(configuration));
    }
}