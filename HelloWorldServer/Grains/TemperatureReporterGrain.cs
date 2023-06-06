using HelloWorldInterfaces;

namespace HelloWorldServer.Grains;

public sealed class TemperatureReporterGrain : Grain, ITemperatureReporterGrainObserver
{
    private readonly ILogger<TemperatureReporterGrain> _logger;

    private readonly string _name;

    public TemperatureReporterGrain(ILogger<TemperatureReporterGrain> logger)
    {
        _logger = logger;

        _name = this.GetPrimaryKeyString();
    }

    public async Task TemperatureNotification(TemperatureData tempData)
    {
        await Task.Delay(1_000);

        _logger.LogInformation("{name} reported the new temperature {value} in {monitorName}.", _name, tempData.TemperatureValue, tempData.MonitorName);
    }
}
