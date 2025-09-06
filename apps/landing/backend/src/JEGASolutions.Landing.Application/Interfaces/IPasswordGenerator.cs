namespace JEGASolutions.Landing.Application.Interfaces;

public interface IPasswordGenerator
{
    string GenerateSecurePassword(int length = 12);
    string GenerateSubdomain(string baseName, int maxLength = 20);
}