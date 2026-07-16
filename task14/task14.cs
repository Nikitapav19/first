using System;
using System.Threading;

namespace task14;

public class DefiniteIntegral
{
    private static double _totalSum;

    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsnumber)
    {
        _totalSum = 0.0;
        Barrier barrier = new Barrier(threadsnumber + 1);
        double range = b - a;
        double chunk = range / threadsnumber;

        for (int i = 0; i < threadsnumber; i++)
        {
            int threadIndex = i;
            
            Thread thread = new Thread(() =>
            {
                double localA = a + threadIndex * chunk;
                double localB = localA + chunk;
                
                if (threadIndex == threadsnumber - 1)
                {
                    localB = b;
                }

                double localSum = 0.0;
                double current = localA;

                while (current < localB)
                {
                    double next = current + step;
                    if (next > localB)
                    {
                        next = localB;
                    }

                    localSum += (function(current) + function(next)) / 2.0 * (next - current);
                    current = next;
                }

                double currentSum = _totalSum;
                double newSum = currentSum + localSum;
                
                while (Interlocked.CompareExchange(ref _totalSum, newSum, currentSum) != currentSum)
                {
                    currentSum = _totalSum;
                    newSum = currentSum + localSum;
                }

                barrier.SignalAndWait();
            });
            
            thread.Start();
        }

        barrier.SignalAndWait();
        return _totalSum;
    }
}