namespace JEGASolutions.ExtraHours.Core.Interfaces
{
    public interface IJWTTokenService
    {
        void InvalidateToken(string token);
        bool IsTokenInvalid(string token);
    }
}
