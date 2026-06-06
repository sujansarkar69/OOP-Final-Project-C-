namespace PasswordResetBruteForce_GUI.Services;

public class PasswordGenerator
{
    private readonly Random _random = new Random();

    public const string CharacterSet = "abc123";

    public string GeneratePassword()
    {
        int length = _random.Next(4, 6); // [4–6), so length is 4 or 5

        char[] password = new char[length];

        for (int i = 0; i < length; i++)
        {
            int index = _random.Next(CharacterSet.Length);
            password[i] = CharacterSet[index];
        }

        return new string(password);
    }
}