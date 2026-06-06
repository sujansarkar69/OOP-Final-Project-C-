namespace PasswordResetBruteForce_GUI.Services;

public class BruteForceGenerator
{
    private readonly string _characters;

    public BruteForceGenerator(string characters)
    {
        _characters = characters;
    }

    public long GetTotalCombinations(int maxLength)
    {
        long total = 0;

        for (int length = 1; length <= maxLength; length++)
        {
            total += (long)Math.Pow(_characters.Length, length);
        }

        return total;
    }

    public string GenerateByIndex(long index)
    {
        int length = 1;
        long countForLength = _characters.Length;

        while (index >= countForLength)
        {
            index -= countForLength;
            length++;
            countForLength = (long)Math.Pow(_characters.Length, length);
        }

        char[] result = new char[length];

        for (int i = length - 1; i >= 0; i--)
        {
            result[i] = _characters[(int)(index % _characters.Length)];
            index /= _characters.Length;
        }

        return new string(result);
    }
}