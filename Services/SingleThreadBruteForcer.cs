using System.Diagnostics;
using PasswordResetBruteForce_GUI.Models;

namespace PasswordResetBruteForce_GUI.Services;

public class SingleThreadBruteForcer
{
    private readonly BruteForceGenerator _generator;
    private readonly PasswordValidator _validator;

    public SingleThreadBruteForcer(BruteForceGenerator generator, PasswordValidator validator)
    {
        _generator = generator;
        _validator = validator;
    }

    public async Task<AttackResult> StartAsync(
        string targetHash,
        int maxLength,
        IProgress<double> progress,
        IProgress<long> attemptsProgress,
        CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            long totalCombinations = _generator.GetTotalCombinations(maxLength);
            long attempts = 0;

            for (long i = 0; i < totalCombinations; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                string candidate = _generator.GenerateByIndex(i);
                attempts++;

                if (_validator.IsValidPassword(candidate, targetHash))
                {
                    stopwatch.Stop();

                    return new AttackResult
                    {
                        IsFound = true,
                        FoundPassword = candidate,
                        Attempts = attempts,
                        ElapsedTime = stopwatch.Elapsed
                    };
                }

                if (attempts % 100 == 0)
                {
                    progress.Report((double)attempts / totalCombinations);
                    attemptsProgress.Report(attempts);
                }
            }

            stopwatch.Stop();

            return new AttackResult
            {
                IsFound = false,
                FoundPassword = "",
                Attempts = attempts,
                ElapsedTime = stopwatch.Elapsed
            };
        });
    }
}