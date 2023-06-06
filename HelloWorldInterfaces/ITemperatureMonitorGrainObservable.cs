namespace HelloWorldInterfaces;

public interface ITemperatureMonitorGrainObservable : IGrainWithStringKey
{
    Task ChangeTemperature(int temperature);

    Task Subscribe(ITemperatureReporterGrainObserver observerReporter);

    Task UnSubscribe(ITemperatureReporterGrainObserver observerReporter);
}
