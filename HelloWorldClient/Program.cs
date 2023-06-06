using HelloWorldInterfaces;

namespace HelloWorldClient;

public static class Program
{
    public static void Main(string[] args)
    {
        IHost host = Host
            .CreateDefaultBuilder(args)
            .UseOrleansClient(configureOrleans)
            .ConfigureServices(configureServices)
            .Build();

        host.Run();
    }

    private static void configureOrleans(IClientBuilder builder)
    {
        builder.UseLocalhostClustering();

        builder.UseTransactions();

        builder.AddMemoryStreams(Constants.StreamProviderName);

        //const string azureStorageConnString = "UseDevelopmentStorage=true";
        //builder.AddAzureQueueStreams(Constants.StreamProviderName, (ClusterClientAzureQueueStreamConfigurator configurator) => configurator.ConfigureAzureQueue(options => options.Configure(aqo => aqo.ConfigureQueueServiceClient(azureStorageConnString))));
    }

    private static void configureServices(IServiceCollection services)
    {
        services.AddHostedService<HelloGrainWorker>();
        services.AddHostedService<MoneyTransferGrainWorker>();
        services.AddHostedService<TemperatureWorker>();
        services.AddHostedService<StreamWorker>();
    }
}