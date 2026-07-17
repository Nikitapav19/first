using System;
using System.Threading;
using Xunit;

public class Task19Tests
{
    [Fact]
    public void Illustrate_LongRunningOperations()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;  // Почему-то консоль по дефолту имеет другую кодировку

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