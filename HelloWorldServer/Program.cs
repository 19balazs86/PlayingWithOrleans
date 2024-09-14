using HelloWorldInterfaces;
using HelloWorldServer.StartupTasks;

namespace HelloWorldServer;

public static class Program
{
    public const string CounterStorageName     = "CounterStore";
    public const string TransactionStorageName = "TransactionStore";
    public const string StreamStorageName      = "PubSubStore";

    public static void Main(string[] args)
    {
        IHost host = Host
            .CreateDefaultBuilder(args)
            .UseOrleans(configureOrleans)
            .Build();

        host.Run();
    }

    private static void configureOrleans(HostBuilderContext builderContext, ISiloBuilder builder)
    {
        bool isDevelopment = builderContext.HostingEnvironment.IsDevelopment();

        IConfiguration configuration = builderContext.Configuration;

        //const string azureStorageConnString = "UseDevelopmentStorage=true";

        if (isDevelopment)
        {
            builder.UseLocalhostClustering();

            // You can add a default storage provider and no need to define the providerName parameter for any IPersistentState
            // builder.AddMemoryGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME);

            builder.AddMemoryGrainStorage(CounterStorageName);
            builder.AddMemoryGrainStorage(TransactionStorageName);
            builder.AddMemoryGrainStorage(StreamStorageName);

            builder.UseInMemoryReminderService();

            builder.AddMemoryStreams(Constants.StreamProviderName);
        }
        else // Production
        {
            // Consider using: Microsoft.Orleans.GrainDirectory.AzureStorage or Redis
            // https://learn.microsoft.com/en-us/dotnet/orleans/host/grain-directory
            // builder.AddAzureTableGrainDirectory("my-grain-directory", options => options.ConnectionString = azureStorageConnString);

            // Install-Package Microsoft.Orleans.Persistence.AzureStorage
            // builder.AddAzureTableGrainStorage(StreamStorageName, options => options.ConfigureTableServiceClient(azureStorageConnString));

            // Install-Package Microsoft.Orleans.Streaming.AzureStorage
            // builder.AddAzureQueueStreams(Constants.StreamProviderName, (SiloAzureQueueStreamConfigurator configurator) => configurator.ConfigureAzureQueue(options => options.Configure(aqo => aqo.ConfigureQueueServiceClient(azureStorageConnString))));
        }

        builder.UseTransactions();

        builder.UseDashboard(); // http://localhost:8080

        builder.AddStartupTask<InitReminderStartupTask>();
    }
}