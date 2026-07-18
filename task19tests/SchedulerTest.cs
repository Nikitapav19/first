using System;
using System.IO;
using System.Threading;
using Xunit;

public static class Config
{
    public static readonly string ReportPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../report.txt"));
}

public class TestCommand : ICommand
{
    private readonly int _id;
    int counter = 0;

    public TestCommand(int id)
    {
        _id = id;
    }

    public void Execute()
    {
        counter++;
        string msg = $"Поток {_id} вызов {counter}\n";
        
        File.AppendAllText(Config.ReportPath, msg); 
    }
}

public class Task19Tests
{
    [Fact]
    public void Illustrate_LongRunningOperations()
    {
        File.WriteAllText(Config.ReportPath, ""); 

        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);
        
        for (int i = 1; i <= 5; i++)
        {
            var testCmd = new TestCommand(i);
            var longCmd = new LongRunningCommand(testCmd, scheduler, 3);
            scheduler.Add(longCmd);
        }

        server.Start();
        Thread.Sleep(500);

        var hardStop = new HardStop(server);
        server.Queue.Add(hardStop);
        server.Thread.Join(1000);

        Assert.True(server.IsStopped);
    }
}