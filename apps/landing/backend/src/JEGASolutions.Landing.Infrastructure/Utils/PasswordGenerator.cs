using JEGASolutions.Landing.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace JEGASolutions.Landing.Infrastructure.Utils;

public class PasswordGenerator : IPasswordGenerator
{
    private readonly string[] _invalidSubdomainWords = { "www", "api", "admin", "mail", "ftp", "blog", "shop", "store" };
    private readonly char[] _specialChars = { '!', '@', '#', '$', '%', '^', '&', '*' };

    public string GenerateSecurePassword(int length = 12)
    {
        var password = new StringBuilder();

        // Ensure at least one of each character type
        password.Append(GenerateRandomChar('A', 'Z')); // Uppercase
        password.Append(GenerateRandomChar('a', 'z')); // Lowercase
        password.Append(GenerateRandomChar('0', '9')); // Number
        password.Append(_specialChars[RandomNumberGenerator.GetInt32(_specialChars.Length)]); // Special

        // Fill remaining length with random characters
        var allChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";

        for (int i = 4; i < length; i++)
        {
            password.Append(allChars[RandomNumberGenerator.GetInt32(allChars.Length)]);
        }

        // Shuffle the password
        return ShuffleString(password.ToString());
    }

    public string GenerateSubdomain(string baseName, int maxLength = 20)
    {
        var cleanName = CleanSubdomainName(baseName);

        // If name is too long, truncate it
        if (cleanName.Length > maxLength - 5) // Leave space for suffix
        {
            cleanName = cleanName.Substring(0, maxLength - 5);
        }

        // Check for reserved words
        if (_invalidSubdomainWords.Contains(cleanName.ToLower()))
        {
            cleanName += "co";
        }

        // Add random suffix to ensure uniqueness
        var randomSuffix = RandomNumberGenerator.GetInt32(1000, 9999);
        var subdomain = $"{cleanName}{randomSuffix}";

        return subdomain.ToLower();
    }

    private string CleanSubdomainName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "cliente";

        var cleaned = new StringBuilder();

        foreach (char c in input.ToLower())
        {
            if (char.IsLetterOrDigit(c))
            {
                cleaned.Append(c);
            }
            else if (c == ' ' || c == '.' || c == '@' || c == '-' || c == '_')
            {
                if (cleaned.Length > 0 && cleaned[cleaned.Length - 1] != '-')
                {
                    cleaned.Append('-');
                }
            }
        }

        // Remove trailing dash
        var result = cleaned.ToString().TrimEnd('-');

        return string.IsNullOrEmpty(result) ? "cliente" : result;
    }

    private char GenerateRandomChar(char min, char max)
    {
        return (char)RandomNumberGenerator.GetInt32(min, max + 1);
    }

    private string ShuffleString(string input)
    {
        var array = input.ToCharArray();

        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }

        return new string(array);
    }
}