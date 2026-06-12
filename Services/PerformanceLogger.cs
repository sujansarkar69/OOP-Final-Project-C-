using PasswordResetBruteForce_GUI.Models;

namespace PasswordResetBruteForce_GUI.Services;

public class PerformanceLogger
{
    private readonly List<string> _logs = new List<string>();

    public void AddLog(string attackType, AttackResult result)
    {
        string log =
            $"{attackType}: Password='{result.FoundPassword}', " +
            $"Attempts={result.Attempts}, " +
            $"Time={result.ElapsedTime.TotalSeconds:F3} seconds";

        _logs.Add(log);
    }

    public string GetLogs()
    {
        return string.Join(Environment.NewLine, _logs);
    }

    public void Clear()
    {
        _logs.Clear();
    }
}