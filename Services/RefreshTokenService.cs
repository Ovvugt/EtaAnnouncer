using EtaAnnouncer.Data;
using EtaAnnouncer.Models;
using Microsoft.EntityFrameworkCore;

namespace EtaAnnouncer.Services
{
    public class RefreshTokenService(IdentityDbContext dbContext)
    {
        public async Task SaveRefreshTokenAsync(string userId, string token)
        {
            var refreshToken = new RefreshToken
            {
                Token = token,
                UserId = userId,
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                IsRevoked = false
            };
            
            await dbContext.RefreshTokens.AddAsync(refreshToken);
            await dbContext.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token) 
            => await dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked && rt.ExpiryDate > DateTime.UtcNow);

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);

            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveExpiredOrRevokedTokensAsync()
        {
            var expiredTokens = await dbContext.RefreshTokens
                .Where(rt => rt.ExpiryDate <= DateTime.UtcNow || rt.IsRevoked)
                .ToListAsync();

            dbContext.RemoveRange(expiredTokens);
            await dbContext.SaveChangesAsync();
        }
    }
}
