using EtaAnnouncer.Models;
using Microsoft.EntityFrameworkCore;

namespace EtaAnnouncer.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Home> Homes { get; set; }
        public DbSet<HomeInvitation> Invitations { get; set; }
    }
}
