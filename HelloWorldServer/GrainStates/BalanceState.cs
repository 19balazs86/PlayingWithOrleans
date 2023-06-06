namespace HelloWorldServer.GrainStates;

[GenerateSerializer]
public sealed record BalanceState
{
    [Id(0)]
    public decimal Value { get; set; } = 1_000;
}
