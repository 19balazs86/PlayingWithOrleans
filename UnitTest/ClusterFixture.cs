using HelloWorldServer;
using Orleans.TestingHost;

namespace UnitTest;

// Note: We can write Integration and Acceptance tests using the TestCluster as well
public sealed class ClusterFixture : IAsyncLifetime
{
    public TestCluster Cluster { get; private set; }

    public ClusterFixture()
    {
        var builder = new TestClusterBuilder();

        builder.AddSiloBuilderConfigurator<TestSiloConfigurator>();

        Cluster = builder.Build();
    }

    public async Task InitializeAsync()
    {
        await Cluster.DeployAsync();
    }

    public async Task DisposeAsync()
    {
        await Cluster.StopAllSilosAsync();
    }
}

public sealed class TestSiloConfigurator : ISiloConfigurator
{
    public void Configure(ISiloBuilder builder)
    {
        builder.AddMemoryGrainStorage(Program.CounterStorageName);
    }
}