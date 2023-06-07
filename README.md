# Playing with Microsoft Orleans

In this repository, I have started an experimentation with Orleans as an Actor Model.

I tested the following features - [Microsoft Orleans documentation](https://learn.microsoft.com/en-us/dotnet/orleans/overview) 📚

- [Persistence](https://learn.microsoft.com/en-us/dotnet/orleans/grains/grain-persistence)
- [Reminder](https://learn.microsoft.com/en-us/dotnet/orleans/grains/timers-and-reminders)
- [Startup tasks](https://learn.microsoft.com/en-us/dotnet/orleans/host/configuration-guide/startup-tasks)
- [Transactions](https://learn.microsoft.com/en-us/dotnet/orleans/grains/transactions) - *[Reentrancy or Interleaving](https://learn.microsoft.com/en-us/dotnet/orleans/grains/reentrancy) for the AccountGrain, [Stateless worker](https://learn.microsoft.com/en-us/dotnet/orleans/grains/stateless-worker-grains) for the MoneyTransferGrain*
- [Observers](https://learn.microsoft.com/en-us/dotnet/orleans/grains/observers)
- [Streaming](https://learn.microsoft.com/en-us/dotnet/orleans/streaming)
- [Unit testing](https://learn.microsoft.com/en-us/dotnet/orleans/implementation/testing)
- [Dashboard](https://github.com/OrleansContrib/OrleansDashboard) 👤*OrleansContrib*

#### Resources

- [Code sample browser](https://learn.microsoft.com/en-us/samples/browse/?filter-products=orle&products=dotnet-orleans) 📚
- [SignalR.Orleans](https://github.com/OrleansContrib/SignalR.Orleans) 👤*OrleansContrib*
- [Articles, videos and samples](https://awesome-architecture.com/actor-model-architecture/orleans) 📓*Awesome Software Architecture*

##### Personal Note

- All in all, I have mixed feelings about Orleans. I believe it is well-suited for games and other scenarios where Actors/Grains can participate as business logic.
- The architecture provides an efficient way of scaling the computation power by adding more Servers -> Silos -> Grains.
- No need to worry about database persistence and caching since Orleans handles it out of the box.
- However, I am not entirely convinced about the use of Reminder and Streams. While they are nice to have, I would prefer to use the official packages for Azure ServiceBus or Amazon SQS.