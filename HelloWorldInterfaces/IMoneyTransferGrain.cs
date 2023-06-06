namespace HelloWorldInterfaces;

public interface IMoneyTransferGrain : IGrainWithIntegerKey
{
    [Transaction(TransactionOption.Create)]
    Task Transfer(string fromId, string toId, decimal amountToTransfer);
}
