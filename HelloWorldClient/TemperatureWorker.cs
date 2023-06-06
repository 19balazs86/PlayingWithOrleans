using HelloWorldInterfaces;

namespace HelloWorldClient;

public sealed class TemperatureWorker : BackgroundService
{
    private readonly IClusterClient _client;

    public TemperatureWorker(IClusterClient client)
    {
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var monitorGrain = await initGrains();

        while (!stoppingToken.IsCancellationRequested)
        {
            await monitorGrain.ChangeTemperature(Random.Shared.Next(-20, 40));

            await Task.Delay(5_000, stoppingToken);
        }
    }

    private async Task<ITemperatureMonitorGrainObservable> initGrains()
    {
        // Creating 1 observable entity
        var monitorGrain = _client.GetGrain<ITemperatureMonitorGrainObservable>("Denver-TempMonitor");

        // Creating 2 observers that are notified by the observable entity
        var reporter1 = _client.GetGrain<ITemperatureReporterGrainObserver>(Constants.SayHelloNames.First());
        var reporter2 = _client.GetGrain<ITemperatureReporterGrainObserver>(Constants.SayHelloNames.Last());

        // The subscription will remain active for a minute
        // due to the expiration time of the ObserverManager (in TemperatureMonitorGrain)
        await monitorGrain.Subscribe(reporter1);
        await monitorGrain.Subscribe(reporter2);

        return monitorGrain;
    }
}
