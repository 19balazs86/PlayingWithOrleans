namespace HelloWorldInterfaces;

[GenerateSerializer]
public readonly record struct TemperatureData(string MonitorName, int TemperatureValue);

public interface ITemperatureReporterGrainObserver : IGrainObserver, IGrainWithStringKey
{
    Task TemperatureNotification(TemperatureData temperatureData);
}
