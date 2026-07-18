using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Xunit;
using ScottPlot;

public class LongRunningCommand : ICommand
{
    private IScheduler _scheduler;
    private int _totalSteps;
    public int Steps { get; private set; }
    
    public int Id { get; }
    private Action<int> _onExecute;

    public LongRunningCommand(IScheduler scheduler, int totalSteps)
    {
        _scheduler = scheduler;
        _totalSteps = totalSteps;
        Steps = 0;
    }

    public LongRunningCommand(IScheduler scheduler, int totalSteps, int id, Action<int> onExecute) 
        : this(scheduler, totalSteps)
    {
        Id = id;
        _onExecute = onExecute;
    }

    public void Execute()
    {
        Steps++;
        
        _onExecute?.Invoke(Id);

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

    [Fact]
    public void Scheduler_GeneratesRoundRobinGraph()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ServerThread(scheduler);

        List<double> xs = new List<double>();
        List<double> ys = new List<double>();
        int currentTick = 0;

        for (int i = 1; i <= 5; i++)
        {
            var cmd = new LongRunningCommand(scheduler, 4, i, id => 
            {
                currentTick++;
                xs.Add(currentTick);
                ys.Add(id);
            });
            
            server.Queue.Add(cmd); 
        }

        var softStop = new SoftStopCommand(server);
        server.Queue.Add(softStop);

        server.Start();
        server.Thread.Join(5000);

        var plt = new Plot();
        
        var scatter = plt.Add.Scatter(xs.ToArray(), ys.ToArray());
        scatter.LineWidth = 0;
        scatter.MarkerSize = 10;
        
        plt.Title("Чередование задач в Round Robin");
        plt.Axes.Bottom.Label.Text = "Порядок выполнения (Тик)";
        plt.Axes.Left.Label.Text = "ID задачи";
        
        plt.Axes.SetLimitsY(0, 6);

        string path = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../../round_robin_graph.png"));
        
        plt.SavePng(path, 800, 400);

        Assert.True(server.IsStopped);
        Assert.True(File.Exists(path));
    }
}