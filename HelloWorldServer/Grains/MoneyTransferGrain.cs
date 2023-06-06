using HelloWorldInterfaces;
using Orleans.Concurrency;

namespace HelloWorldServer.Grains;

[StatelessWorker]
public sealed class MoneyTransferGrain : Grain, IMoneyTransferGrain
{
    private readonly IGrainFactory _grainFactory;

    public MoneyTransferGrain(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    // The transaction is created automatically as the interface method has the attribute
    public async Task Transfer(string fromId, string toId, decimal amountToTransfer)
    {
        // It is better to use the injected version as it is easier to mock and write tests
        //IAccountGrain accountGrainFrom = GrainFactory.GetGrain<IAccountGrain>(fromId);

        IAccountGrain accountGrainFrom = _grainFactory.GetGrain<IAccountGrain>(fromId);
        IAccountGrain accountGrainTo   = _grainFactory.GetGrain<IAccountGrain>(toId);

        Task task1 = accountGrainFrom.Withdraw(amountToTransfer);
        Task task2 = accountGrainTo.Deposit(amountToTransfer);

        await Task.WhenAll(task1, task2);
    }
}
