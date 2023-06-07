using HelloWorldInterfaces;

namespace HelloWorldClient.Workers;

public sealed class HelloGrainWorker : BackgroundService
{
    private readonly ILogger<HelloGrainWorker> _logger;
    private readonly IClusterClient _client;

    private readonly GrainCancellationTokenSource _grainCTS;

    public HelloGrainWorker(ILogger<HelloGrainWorker> logger, IClusterClient client)
    {
        _logger = logger;
        _client = client;

        _grainCTS = new GrainCancellationTokenSource();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var disposable = stoppingToken.Register(() => _grainCTS.Cancel().GetAwaiter().GetResult());

        while (!stoppingToken.IsCancellationRequested)
        {
            // There is no race condition for the same Grain. "Emma" appears twice in the list.
            // You can call even multiple methods of a Grain, which are processed one by one.
            // However, processing does occur in concurrently between different Grains.

            // 1) Stateless worker Grains can perform requests concurrently.
            // 2) Interleaving methods can perform requests concurrently.
            await Parallel.ForEachAsync(Constants.SayHelloNames, sayHello);

            await Task.Delay(1_000, stoppingToken);
        }
    }

    private async ValueTask sayHello(string name, CancellationToken cancellation)
    {
        IHelloGrain helloGrain = _client.GetGrain<IHelloGrain>(name);

        string response = await helloGrain.SayHello($"Hello {name}.", _grainCTS.Token);

        _logger.LogInformation("Worker response: '{response}'", response);
    }
}
