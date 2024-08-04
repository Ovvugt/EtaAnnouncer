using EtaAnnouncer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EtaAnnouncer.Data
{
    public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
