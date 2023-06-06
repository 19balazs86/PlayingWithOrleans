namespace HelloWorldServer.GrainStates;

[Serializable]
public sealed class NameCounterState
{
    public int Counter { get; set; } = 0;

    public Dictionary<string, int> ReminderCounter { get; set; } = new();
}