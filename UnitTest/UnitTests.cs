using Orleans.TestingHost;

namespace UnitTest;

public sealed class UnitTests : IClassFixture<ClusterFixture>
{
    private readonly ClusterFixture _clusterFixture;

    private TestCluster _cluster => _clusterFixture.Cluster;

    public UnitTests(ClusterFixture clusterFixture)
    {
        _clusterFixture = clusterFixture;
    }

    [Fact]
    public async Task Test1()
    {
        // Arrange
        const string greeting = "Hello, World";

        IHelloGrain helloGrain = _cluster.GrainFactory.GetGrain<IHelloGrain>("Name");

        using var gcts = new GrainCancellationTokenSource();

        // Act
        string response = await helloGrain.SayHello(greeting, gcts.Token);

        // Assert
        Assert.Contains(greeting, response);
    }
}