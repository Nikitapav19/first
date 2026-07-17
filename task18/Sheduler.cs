using System;
using System.Collections.Concurrent;
using System.Threading;

public interface ICommand
{
    void Execute();
}

public interface IScheduler
{
    bool HasCommand();
    ICommand Select();
    void Add(ICommand cmd);
}

public class RoundRobinScheduler : IScheduler
{
    private readonly ConcurrentQueue<ICommand> _commands = new ConcurrentQueue<ICommand>();
    public AutoResetEvent SyncEvent { get; set; }

    public bool HasCommand()
    {
        return !_commands.IsEmpty;
    }

    public ICommand Select()
    {
        _commands.TryDequeue(out var cmd);
        return cmd;
    }

    public void Add(ICommand cmd)
    {
        _commands.Enqueue(cmd);
        SyncEvent?.Set();
    }
}

public class ThreadQueue
{
    private readonly ConcurrentQueue<ICommand> _queue = new ConcurrentQueue<ICommand>();
    private readonly AutoResetEvent _syncEvent;
    public bool IsAddingCompleted { get; private set; }

    public ThreadQueue(AutoResetEvent syncEvent)
    {
        _syncEvent = syncEvent;
    }

    public void Add(ICommand cmd)
    {
        _queue.Enqueue(cmd);
        _syncEvent.Set();
    }

    public bool TryTake(out ICommand cmd)
    {
        return _queue.TryDequeue(out cmd);
    }

    public bool IsEmpty => _queue.IsEmpty;

    public void CompleteAdding()
    {
        IsAddingCompleted = true;
        _syncEvent.Set();
    }
}

public static class ExceptionHandler
{
    public static void Handle(Exception ex, ICommand command)
    {
    }
}

public class ServerThread
{
    public ThreadQueue Queue { get; }
    public IScheduler Scheduler { get; }
    public Thread Thread { get; }
    public Action Behavior { get; set; }
    public bool IsStopped { get; private set; }

    public readonly AutoResetEvent SyncEvent = new AutoResetEvent(false);
    private bool _takeFromSchedulerNext = false;

    public ServerThread(IScheduler scheduler)
    {
        Queue = new ThreadQueue(SyncEvent);
        Scheduler = scheduler;
        
        if (Scheduler is RoundRobinScheduler rrScheduler)
        {
            rrScheduler.SyncEvent = SyncEvent;
        }

        Behavior = DefaultBehavior;
        Thread = new Thread(Run);
    }

    private void Run()
    {
        while (!IsStopped)
        {
            Behavior();
        }
    }

    public bool TryGetNextCommand(out ICommand cmd)
    {
        cmd = null;
        bool hasSchedulerCmd = Scheduler.HasCommand();
        bool hasQueueCmd = !Queue.IsEmpty;

        if (hasSchedulerCmd && hasQueueCmd)
        {
            if (_takeFromSchedulerNext)
            {
                cmd = Scheduler.Select();
                _takeFromSchedulerNext = false;
            }
            else
            {
                Queue.TryTake(out cmd);
                _takeFromSchedulerNext = true;
            }
        }
        else if (hasSchedulerCmd)
        {
            cmd = Scheduler.Select();
        }
        else if (hasQueueCmd)
        {
            Queue.TryTake(out cmd);
        }

        return cmd != null;
    }

    private void DefaultBehavior()
    {
        while (!Scheduler.HasCommand() && Queue.IsEmpty && !IsStopped && !Queue.IsAddingCompleted)
        {
            SyncEvent.WaitOne();
        }

        if (IsStopped) return;

        if (TryGetNextCommand(out ICommand cmd))
        {
            try
            {
                cmd.Execute();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, cmd);
            }
        }
        else if (Queue.IsAddingCompleted)
        {
            Stop();
        }
    }

    public void Start()
    {
        Thread.Start();
    }

    public void Stop()
    {
        IsStopped = true;
        SyncEvent.Set();
    }
}

public class HardStopCommand : ICommand
{
    private readonly ServerThread _serverThread;

    public HardStopCommand(ServerThread serverThread)
    {
        _serverThread = serverThread;
    }

    public void Execute()
    {
        if (Thread.CurrentThread != _serverThread.Thread)
        {
            throw new InvalidOperationException();
        }

        _serverThread.Stop();
    }
}

public class SoftStopCommand : ICommand
{
    private readonly ServerThread _serverThread;

    public SoftStopCommand(ServerThread serverThread)
    {
        _serverThread = serverThread;
    }

    public void Execute()
    {
        if (Thread.CurrentThread != _serverThread.Thread)
        {
            throw new InvalidOperationException();
        }

        _serverThread.Queue.CompleteAdding();
        _serverThread.Behavior = SoftStopCommandBehavior;
    }

    private void SoftStopCommandBehavior()
    {
        if (_serverThread.TryGetNextCommand(out ICommand cmd))
        {
            try
            {
                cmd.Execute();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, cmd);
            }
        }
        else
        {
            _serverThread.Stop();
        }
    }
}