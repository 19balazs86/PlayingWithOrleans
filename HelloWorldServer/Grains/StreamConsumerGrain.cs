using HelloWorldInterfaces;
using Orleans.Streams;
using Orleans.Streams.Core;

namespace HelloWorldServer.Grains;

// This consumer grain is activated on the silo, and is logging when it is receiving event
[ImplicitStreamSubscription(Constants.StreamNamespace)]
public sealed class StreamConsumerGrain : Grain, IStreamConsumerGrain, IStreamSubscriptionObserver
{
    private readonly ILogger<StreamConsumerGrain> _logger;

    private readonly LoggerObserver _observer;

    public StreamConsumerGrain(ILogger<StreamConsumerGrain> logger)
    {
        _logger = logger;

        _observer = new LoggerObserver(_logger, this.GetPrimaryKey());
    }

    // Called when a subscription is added
    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        // Plug our LoggerObserver to the stream

        StreamSubscriptionHandle<int> handle = handleFactory.Create<int>();

        await handle.ResumeAsync(_observer);
    }
}

public class LoggerObserver : IAsyncObserver<int>
{
    private readonly ILogger _logger;
    private readonly Guid _consumerId;

    public LoggerObserver(ILogger logger, Guid consumerId)
    {
        _logger = logger;
        _consumerId = consumerId;
    }

    public async Task OnNextAsync(int item, StreamSequenceToken? token = null)
    {
        await Task.Delay(5_000);

        _logger.LogInformation("OnNext: Item: {Item}, Token = {Token}, Consumer: {Id}", item, token, _consumerId);
    }

    public Task OnCompletedAsync()
    {
        _logger.LogInformation("OnCompleted Consumer({Id})", _consumerId);

        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        _logger.LogInformation("OnError: {Exception}", ex);

        return Task.CompletedTask;
    }
}
