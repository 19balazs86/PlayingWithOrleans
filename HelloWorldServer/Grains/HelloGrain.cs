using HelloWorldInterfaces;
using HelloWorldServer.GrainStates;
using Orleans.Runtime;

namespace HelloWorldServer.Grains;

[GrainType(GrainTypeName)]
public sealed class HelloGrain : Grain, IHelloGrain
{
    public const string GrainTypeName = nameof(HelloGrain);

    private readonly ILogger<HelloGrain> _logger;

    private readonly IPersistentState<NameCounterState> _counterState;

    private readonly string _name;

    public HelloGrain(
        ILogger<HelloGrain> logger,
        [PersistentState(nameof(NameCounterState), Program.CounterStorageName)]
        IPersistentState<NameCounterState> counterState)
    {
        _logger       = logger;
        _counterState = counterState;

        _name = this.GetPrimaryKeyString(); // GrainReference.GrainId.Key.ToString() | this.GetGrainId().Key.ToString();
    }

    public async Task<string> SayHello(string greeting, GrainCancellationToken grainCancellationToken)
    {
        await Task.Delay(1_000, grainCancellationToken.CancellationToken);

        int counter = ++_counterState.State.Counter;

        _logger.LogInformation("SayHello to '{name}', Counter: {counter}, Greeting = '{Greeting}'", _name, counter, greeting);

        await _counterState.WriteStateAsync();

        return $"Client said: '{greeting}', to {_name}, counter: {counter}";
    }

    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        await Task.Delay(500);

        var reminderCounter = _counterState.State.ReminderCounter;

        if (!reminderCounter.ContainsKey(reminderName))
            reminderCounter.Add(reminderName, 0);

        int counter = ++reminderCounter[reminderName];

        _logger.LogInformation("ReceiveReminder({reminderName}) for {name}, Counter: {counter}", reminderName, _name, counter);

        await _counterState.WriteStateAsync();
    }

    public static GrainId CreateGrainId(string stringKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(stringKey);

        // I use a custom GrainType name, defined with the GrainTypeAttribute on the class
        return GrainId.Create(GrainTypeName, stringKey);
    }
}
