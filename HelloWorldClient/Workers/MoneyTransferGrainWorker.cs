using HelloWorldInterfaces;
using Orleans.Transactions;

namespace HelloWorldClient.Workers;

public sealed class MoneyTransferGrainWorker : BackgroundService
{
    private readonly ILogger<MoneyTransferGrainWorker> _logger;
    private readonly IClusterClient _client;
    private readonly ITransactionClient _transactionClient;

    public MoneyTransferGrainWorker(
        ILogger<MoneyTransferGrainWorker> logger,
        IClusterClient client,
        ITransactionClient transactionClient)
    {
        _logger = logger;
        _client = client;
        _transactionClient = transactionClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await transferMoney_CreateTransactionManually();

            await Task.Delay(1_000, stoppingToken);

            await transferMoney_with_MoneyTransferGrain();

            await Task.Delay(1_000, stoppingToken);
        }
    }

    private async Task transferMoney_CreateTransactionManually()
    {
        var transferAccouns = get_FromId_ToId();

        IAccountGrain fromAccount = _client.GetGrain<IAccountGrain>(transferAccouns.FromId);
        IAccountGrain toAccount = _client.GetGrain<IAccountGrain>(transferAccouns.ToId);

        int transferAmount = Random.Shared.Next(200);

        // --> Option #1 - Create transaction manually
        try
        {
            await _transactionClient.RunTransaction(
                TransactionOption.Create,
                async () =>
                {
                    await fromAccount.Withdraw(transferAmount);
                    await toAccount.Deposit(transferAmount);
                });
        }
        catch (OrleansTransactionAbortedException ex)
        {
            string errorMessage = $"""
                Falied transferring credits from {transferAccouns.FromId} to {transferAccouns.ToId}.
                {ex.Message}
                {ex.InnerException?.Message}
                """;

            _logger.LogError(errorMessage);
        }

        await displayBalance(fromAccount, toAccount, transferAmount);
    }

    private async Task transferMoney_with_MoneyTransferGrain()
    {
        var transferAccouns = get_FromId_ToId();

        // MoneyTransferGrain is a StatelessWorker, can be used multiple times simultaneously.
        IMoneyTransferGrain moneyTransferGrain = _client.GetGrain<IMoneyTransferGrain>(0);

        int transferAmount = Random.Shared.Next(200);

        // --> Option #2 - Transaction is created via MoneyTransferGrain
        try
        {
            await moneyTransferGrain.Transfer(transferAccouns.FromId, transferAccouns.ToId, transferAmount);
        }
        catch (OrleansTransactionAbortedException ex)
        {
            string errorMessage = $"""
                Falied transferring credits from {transferAccouns.FromId} to {transferAccouns.ToId}.
                {ex.Message}
                {ex.InnerException?.Message}
                """;
            // InnerException is InvalidOperationException throw in AccountGrain.Withdraw

            _logger.LogError(errorMessage);
        }

        IAccountGrain fromAccount = _client.GetGrain<IAccountGrain>(transferAccouns.FromId);
        IAccountGrain toAccount = _client.GetGrain<IAccountGrain>(transferAccouns.ToId);

        await displayBalance(fromAccount, toAccount, transferAmount);
    }

    private (string FromId, string ToId) get_FromId_ToId()
    {
        (string fromId, string toId) = (Constants.GetRandomName(), Constants.GetRandomName());

        while (fromId.Equals(toId))
            toId = Constants.GetRandomName();

        return (fromId, toId);
    }

    private async Task displayBalance(IAccountGrain fromAccount, IAccountGrain toAccount, int transferAmount)
    {
        decimal fromBalance = await fromAccount.GetBalance();
        decimal toBalance = await toAccount.GetBalance();

        _logger.LogInformation("Transferred {amount} credits from {fromId} to {toId}", transferAmount, fromAccount.GetPrimaryKeyString(), toAccount.GetPrimaryKeyString());
        _logger.LogInformation("FromBalance: {fromBalance} | ToBalance: {toBalance}", fromBalance, toBalance);
    }
}
