namespace HelloWorldInterfaces;

public interface IHelloGrain : IRemindable, IGrainWithStringKey
{
    Task<string> SayHello(string greeting, GrainCancellationToken grainCancellationToken);
}
