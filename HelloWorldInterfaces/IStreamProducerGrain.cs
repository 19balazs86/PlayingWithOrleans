namespace HelloWorldInterfaces;

public interface IStreamProducerGrain : IGrainWithStringKey
{
    Task StartProducing(string ns, string key);

    Task StopProducing();
}
