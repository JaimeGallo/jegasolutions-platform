using System.Collections.Concurrent;
using JEGASolutions.ExtraHours.Core.Interfaces;

namespace JEGASolutions.ExtraHours.Infrastructure.Services
{
    public class JWTTokenService : IJWTTokenService
    {
        private readonly ConcurrentDictionary<string, bool> _invalidTokens = new();

        public void InvalidateToken(string token)
        {
            _invalidTokens.TryAdd(token, true);
        }

        public bool IsTokenInvalid(string token)
        {
            return _invalidTokens.ContainsKey(token);
        }
    }
}
