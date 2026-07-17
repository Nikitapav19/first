using System;
using System.Collections.Concurrent;
using System.Threading;

public interface ICommand
{
    void Execute();
}

public static class ExceptionHandler
{
    public static void Handle(Exception ex, ICommand command)
    {
    }
}

public class ServerThread
{
    public BlockingCollection<ICommand> Queue { get; }
    public Thread Thread { get; }
    public Action Behavior { get; set; }
    public bool IsStopped { get; private set; }

    public ServerThread()
    {
        Queue = new BlockingCollection<ICommand>();
            
        Behavior = () =>
        {
            try
            {
                var cmd = Queue.Take();
                try
                {
                    cmd.Execute();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex, cmd);
                }
            }
            catch (InvalidOperationException)
            {
                Stop();
            }
        };

        Thread = new Thread(() =>
        {
            while (!IsStopped)
            {
                Behavior();
            }
        });
    }

    public void Start()
    {
        Thread.Start();
    }

    public void Stop()
    {
        IsStopped = true;
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
            
        _serverThread.Behavior = () =>
        {
            if (_serverThread.Queue.TryTake(out var cmd))
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
        };
    }
}