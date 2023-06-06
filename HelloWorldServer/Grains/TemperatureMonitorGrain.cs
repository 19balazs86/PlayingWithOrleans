using HelloWorldInterfaces;
using Orleans.Utilities;

namespace HelloWorldServer.Grains;

public sealed class TemperatureMonitorGrain : Grain, ITemperatureMonitorGrainObservable
{
    private readonly ILogger<TemperatureMonitorGrain> _logger;

    private readonly ObserverManager<ITemperatureReporterGrainObserver> _observerManager;

    private readonly string _name;

    public TemperatureMonitorGrain(ILogger<TemperatureMonitorGrain> logger)
    {
        _logger = logger;

        // Observers get reported for 1 minute
        _observerManager = new ObserverManager<ITemperatureReporterGrainObserver>(TimeSpan.FromMinutes(1), logger);

        _name = this.GetPrimaryKeyString();
    }

    public async Task ChangeTemperature(int temperature)
    {
        var tempData = new TemperatureData(_name, temperature);

        // Notify the observers 1 by 1
        // IMPORTANT: ObserverManager will remove the observer in the event of a failure, see it in the Notify method
        await _observerManager.Notify(observer => observer.TemperatureNotification(tempData));

        _logger.LogInformation("TempMonitor({name}) notified all observers.", _name);
    }

    public Task Subscribe(ITemperatureReporterGrainObserver observerReporter)
    {
        _observerManager.Subscribe(observerReporter, observerReporter);

        return Task.CompletedTask;
    }

    public Task UnSubscribe(ITemperatureReporterGrainObserver observerReporter)
    {
        _observerManager.Unsubscribe(observerReporter);

        return Task.CompletedTask;
    }
}
