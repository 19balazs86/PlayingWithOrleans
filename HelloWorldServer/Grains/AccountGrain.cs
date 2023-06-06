using HelloWorldInterfaces;
using HelloWorldServer.GrainStates;
using Orleans.Concurrency;
using Orleans.Transactions.Abstractions;

namespace HelloWorldServer.Grains;

[Reentrant]
public sealed class AccountGrain : Grain, IAccountGrain
{
    private readonly ILogger<AccountGrain> _logger;
    private readonly ITransactionalState<BalanceState> _balanceState;

    private readonly string _accountName;

    public AccountGrain(
        ILogger<AccountGrain> logger,
        [TransactionalState(nameof(BalanceState), Program.TransactionStorageName)]
        ITransactionalState<BalanceState> balanceState)
    {
        _logger       = logger;
        _balanceState = balanceState;

        _accountName = this.GetPrimaryKeyString();
    }

    public async Task<decimal> GetBalance()
    {
        return await _balanceState.PerformRead(balance => balance.Value);
    }

    public async Task Deposit(decimal amount)
    {
        _logger.LogInformation("Make a deposit for {account} with amount: {amount}", _accountName, amount);

        await _balanceState.PerformUpdate(balance => balance.Value += amount);
    }

    public async Task Withdraw(decimal amount)
    {
        _logger.LogInformation("Withdraw from {account} with amount: {amount}", _accountName, amount);

        await _balanceState.PerformUpdate(balance =>
        {
            if (balance.Value < amount)
            {
                throw new InvalidOperationException(
                    $"Withdrawing {amount} credits from account({_accountName}) would overdraw it. This account has {balance.Value} credits.");
            }

            balance.Value -= amount;
        });
    }
}
