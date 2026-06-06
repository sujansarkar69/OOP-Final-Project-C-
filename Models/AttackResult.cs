namespace PasswordResetBruteForce_GUI.Models;

public class AttackResult
{
    public bool IsFound { get; set; }
    public string FoundPassword { get; set; } = "";
    public long Attempts { get; set; }
    public TimeSpan ElapsedTime { get; set; }
}