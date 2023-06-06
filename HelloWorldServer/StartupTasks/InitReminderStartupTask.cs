using HelloWorldInterfaces;
using HelloWorldServer.Grains;
using Orleans.Runtime;
using Orleans.Timers;

namespace HelloWorldServer.StartupTasks;

public sealed class InitReminderStartupTask : IStartupTask
{
    private readonly TimeSpan _dueTime = TimeSpan.FromSeconds(5);
    private readonly TimeSpan _period  = TimeSpan.FromMinutes(1); // Minimum allowed reminder period is 1 minute!

    private readonly IGrainFactory _grainFactory;
    private readonly IReminderRegistry _reminderRegistry;

    public InitReminderStartupTask(IGrainFactory grainFactory, IReminderRegistry reminderRegistry)
    {
        _grainFactory     = grainFactory;
        _reminderRegistry = reminderRegistry;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        string name = Constants.SayHelloNames.Last();

        //IHello helloGrain = _grainFactory.GetGrain<IHello>(name);

        GrainId grainId = HelloGrain.CreateGrainId(name); //helloGrain.GetGrainId();

        // After 5 seconds, this will trigger the reminder every minute.
        _ = await _reminderRegistry.RegisterOrUpdateReminder(grainId, "Reminder_1", _dueTime, _period);
        _ = await _reminderRegistry.RegisterOrUpdateReminder(grainId, "Reminder_2", _dueTime, _period);

        // _reminderRegistry.UnregisterReminder(...)
        // _reminderRegistry.GetReminders(...)
    }
}
