using HelloWorldInterfaces;
using Orleans.Streams;

namespace HelloWorldServer.Grains;

public sealed class StreamProducerGrain : Grain, IStreamProducerGrain
{
    private readonly GrainTimerCreationOptions _timerOptions = new() { DueTime = TimeSpan.Zero, Period = TimeSpan.FromSeconds(3), KeepAlive = true };

    private readonly ILogger<StreamProducerGrain> _logger;

    private IAsyncStream<int>? _stream;

    private IGrainTimer? _disposableTimer;

    private int _counter = 0;

    public StreamProducerGrain(ILogger<StreamProducerGrain> logger)
    {
        _logger = logger;
    }

    public Task StartProducing(string ns, string key)
    {
        if (_disposableTimer is not null)
            throw new Exception("This grain is already producing events.");

        StreamId streamId = StreamId.Create(ns, key);

        _stream = this.GetStreamProvider(Constants.StreamProviderName).GetStream<int>(streamId);

        _disposableTimer = this.RegisterGrainTimer(timerCallBack, _timerOptions);

        _logger.LogInformation("I will produce a new event every {Period}.", _timerOptions.Period);

        return Task.CompletedTask;
    }

    public Task StopProducing()
    {
        _disposableTimer?.Dispose();

        _disposableTimer = null;
        _stream          = null;

        _logger.LogInformation("Stop producing event.");

        return Task.CompletedTask;
    }

    private async Task timerCallBack(CancellationToken ct = default)
    {
        _counter++;

        _logger.LogInformation("Sending event. Number: {Number}", _counter);

        if (_stream is not null)
            await _stream!.OnNextAsync(_counter);
    }
}
