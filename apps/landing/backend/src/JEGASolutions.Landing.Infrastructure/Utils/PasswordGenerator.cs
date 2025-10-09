using JEGASolutions.Landing.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;

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

        // ✅ PASO 1: Normalizar y remover acentos
        var normalized = RemoveAccents(input);

        var cleaned = new StringBuilder();

        // ✅ PASO 2: Limpiar caracteres especiales
        foreach (char c in normalized.ToLower())
        {
            // Solo permitir letras ASCII (a-z) y dígitos (0-9)
            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
            {
                cleaned.Append(c);
            }
            else if (c == ' ' || c == '.' || c == '@' || c == '-' || c == '_')
            {
                // Convertir espacios y caracteres especiales en guiones
                if (cleaned.Length > 0 && cleaned[cleaned.Length - 1] != '-')
                {
                    cleaned.Append('-');
                }
            }
            // ✅ Todos los demás caracteres se ignoran (emojis, símbolos, etc.)
        }

        // Remove trailing dash
        var result = cleaned.ToString().TrimEnd('-');

        return string.IsNullOrEmpty(result) ? "cliente" : result;
    }

    /// <summary>
    /// Remueve acentos y diacríticos de caracteres Unicode.
    /// Ejemplos: é→e, ñ→n, ü→u, á→a, ç→c
    /// </summary>
    private string RemoveAccents(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        // Normalizar a FormD (descomponer caracteres compuestos)
        // Ejemplo: 'é' se descompone en 'e' + acento combinado
        var normalizedString = input.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (char c in normalizedString)
        {
            // Filtrar solo caracteres que NO sean marcas diacríticas
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        // Normalizar de vuelta a FormC (composición canónica)
        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
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
