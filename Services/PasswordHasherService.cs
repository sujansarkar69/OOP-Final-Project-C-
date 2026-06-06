using System.Security.Cryptography;
using System.Text;

namespace PasswordResetBruteForce_GUI.Services;

public class PasswordHasherService
{
    public const string StaticSalt = "OOP_STATIC_SALT_2026";

    public string HashPassword(string password)
    {
        string saltedPassword = StaticSalt + password;

        byte[] bytes = Encoding.UTF8.GetBytes(saltedPassword);
        byte[] hashBytes = SHA256.HashData(bytes);

        StringBuilder builder = new StringBuilder();

        foreach (byte b in hashBytes)
        {
            builder.Append(b.ToString("x2"));
        }

        return builder.ToString();
    }
}