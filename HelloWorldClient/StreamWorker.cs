using HelloWorldInterfaces;
using Orleans.Runtime;
using Orleans.Streams;

namespace HelloWorldClient;

public sealed class StreamWorker : IHostedService
{
    private readonly ILogger<StreamWorker> _logger;
    private readonly IClusterClient _client;

    private IStreamProducerGrain? _producerGrain;

    private StreamSubscriptionHandle<int>? _streamSubscription;

    public StreamWorker(ILogger<StreamWorker> logger, IClusterClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        string streamKey = Guid.NewGuid().ToString("N");

        await initSiloProducer(streamKey);

        await initClientSubscription(streamKey);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // Grain can produce events even if the client is not connected, so there is no need to stop it
        if (_producerGrain is not null)
            await _producerGrain.StopProducing();

        if (_streamSubscription is not null)
            await _streamSubscription.UnsubscribeAsync();
    }

    private async Task initSiloProducer(string streamKey)
    {
        // Use the connected client to ask a grain to start producing events
        _producerGrain = _client.GetGrain<IStreamProducerGrain>("MyProducer");

        await _producerGrain.StartProducing(Constants.StreamNamespace, streamKey);
    }

    private async Task initClientSubscription(string streamKey)
    {
        // Client can also explicitly subscribe to streams
        StreamId streamId = StreamId.Create(Constants.StreamNamespace, streamKey);

        IAsyncStream<int> stream = _client.GetStreamProvider(Constants.StreamProviderName).GetStream<int>(streamId);

        _streamSubscription = await stream.SubscribeAsync(onNext);
    }

    private async Task onNext(int item, StreamSequenceToken? token = null)
    {
        await Task.Delay(5_000);

        _logger.LogInformation("Stream.OnNext: Item: {Item}, Token = {Token}", item, token);
    }
}
