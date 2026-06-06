namespace PasswordResetBruteForce_GUI.Services;

public class PasswordValidator
{
    private readonly PasswordHasherService _hasher;

    public PasswordValidator(PasswordHasherService hasher)
    {
        _hasher = hasher;
    }

    public bool IsValidPassword(string candidatePassword, string targetHash)
    {
        string candidateHash = _hasher.HashPassword(candidatePassword);
        return candidateHash == targetHash;
    }
}