namespace HelloWorldInterfaces;

// Serializing record types -> https://learn.microsoft.com/en-us/dotnet/orleans/host/configuration-guide/serialization#serializing-record-types
[GenerateSerializer]
public readonly record struct TemperatureData(string MonitorName, int TemperatureValue);

public interface ITemperatureReporterGrainObserver : IGrainObserver, IGrainWithStringKey
{
    Task TemperatureNotification(TemperatureData temperatureData);
}
