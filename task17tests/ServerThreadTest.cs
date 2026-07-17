using System;
using System.Threading;
using Xunit;

public class DummyCommand : ICommand
{
    public bool IsExecuted { get; private set; }
    
    public void Execute()
    {
        IsExecuted = true;
    }
}

public class ServerThreadTests
{
    [Fact]
    public void HardStop_StopsImmediately()
    {
        var server = new ServerThread();
        var cmd1 = new DummyCommand();
        var cmd2 = new DummyCommand();
        var hardStop = new HardStopCommand(server);

        server.Queue.Add(hardStop);
        server.Queue.Add(cmd1);
        server.Queue.Add(cmd2);

        server.Start();
        server.Thread.Join(1000);

        Assert.True(server.IsStopped);
        Assert.False(cmd1.IsExecuted);
        Assert.False(cmd2.IsExecuted);
    }

    [Fact]
    public void SoftStop_ExecutesRemaining()
    {
        var server = new ServerThread();
        var cmd1 = new DummyCommand();
        var cmd2 = new DummyCommand();
        var softStop = new SoftStopCommand(server);

        server.Queue.Add(softStop);
        server.Queue.Add(cmd1);
        server.Queue.Add(cmd2);

        server.Start();
        server.Thread.Join(1000);

        Assert.True(server.IsStopped);
        Assert.True(cmd1.IsExecuted);
        Assert.True(cmd2.IsExecuted);
    }

    [Fact]
    public void HardStop_WrongThread_Throws()
    {
        var server = new ServerThread();
        var hardStop = new HardStopCommand(server);

        Assert.Throws<InvalidOperationException>(() => hardStop.Execute());
    }

    [Fact]
    public void SoftStop_WrongThread_Throws()
    {
        var server = new ServerThread();
        var softStop = new SoftStopCommand(server);

        Assert.Throws<InvalidOperationException>(() => softStop.Execute());
    }
}