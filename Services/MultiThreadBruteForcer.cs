using System.Diagnostics;
using PasswordResetBruteForce_GUI.Models;

namespace PasswordResetBruteForce_GUI.Services;

public class MultiThreadBruteForcer
{
    private readonly BruteForceGenerator _generator;
    private readonly PasswordValidator _validator;

    public MultiThreadBruteForcer(BruteForceGenerator generator, PasswordValidator validator)
    {
        _generator = generator;
        _validator = validator;
    }

    public async Task<AttackResult> StartAsync(
        string targetHash,
        int maxLength,
        int threadCount,
        IProgress<double> progress,
        IProgress<long> attemptsProgress,
        CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        long totalCombinations = _generator.GetTotalCombinations(maxLength);
        long attempts = 0;

        string foundPassword = "";
        bool isFound = false;

        int maxThreads = Math.Max(1, Environment.ProcessorCount);
        threadCount = Math.Clamp(threadCount, 1, maxThreads);

        using CancellationTokenSource linkedCts =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        List<Task> tasks = new List<Task>();

        for (int threadIndex = 0; threadIndex < threadCount; threadIndex++)
        {
            int localThreadIndex = threadIndex;

            Task task = Task.Run(() =>
            {
                for (long i = localThreadIndex; i < totalCombinations; i += threadCount)
                {
                    if (linkedCts.Token.IsCancellationRequested)
                    {
                        break;
                    }

                    string candidate = _generator.GenerateByIndex(i);

                    long currentAttempts = Interlocked.Increment(ref attempts);

                    if (_validator.IsValidPassword(candidate, targetHash))
                    {
                        foundPassword = candidate;
                        isFound = true;
                        linkedCts.Cancel();
                        break;
                    }

                    if (currentAttempts % 100 == 0)
                    {
                        progress.Report((double)currentAttempts / totalCombinations);
                        attemptsProgress.Report(currentAttempts);
                    }
                }
            }, linkedCts.Token);

            tasks.Add(task);
        }

        try
        {
            await Task.WhenAll(tasks);
        }
        catch
        {
            // Some tasks may stop because cancellation was requested.
        }

        stopwatch.Stop();

        return new AttackResult
        {
            IsFound = isFound,
            FoundPassword = foundPassword,
            Attempts = attempts,
            ElapsedTime = stopwatch.Elapsed
        };
    }
}