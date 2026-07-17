using System;
using System.Threading;
using Xunit;

public class LongRunningCommand : ICommand
{
    private IScheduler _scheduler;
    private int _totalSteps;
    public int Steps { get; private set; }

    public LongRunningCommand(IScheduler scheduler, int totalSteps)
    {
        _scheduler = scheduler;
        _totalSteps = totalSteps;
        Steps = 0;
    }

    public void Execute()
    {
        Steps++;
        if (Steps < _totalSteps)
        {
            _scheduler.Add(this);
        }
    }
}

public class ServerThreadSchedulerTests
{
    [Fact]
    public void Scheduler_ExecutesLongCommandToTheEnd()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        var longCmd = new LongRunningCommand(scheduler, 5);
        var softStop = new SoftStopCommand(server);

        server.Queue.Add(longCmd);
        server.Queue.Add(softStop);

        server.Start();
        server.Thread.Join(2000);

        Assert.True(server.IsStopped);
        Assert.Equal(5, longCmd.Steps);
    }

    [Fact]
    public void Scheduler_SwitchesBetweenCommands()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        var cmd1 = new LongRunningCommand(scheduler, 2);
        var cmd2 = new LongRunningCommand(scheduler, 2);
        var softStop = new SoftStopCommand(server);

        server.Queue.Add(cmd1);
        server.Queue.Add(cmd2);
        server.Queue.Add(softStop);

        server.Start();
        server.Thread.Join(2000);

        Assert.Equal(2, cmd1.Steps);
        Assert.Equal(2, cmd2.Steps);
    }
}