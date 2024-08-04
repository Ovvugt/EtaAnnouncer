using Microsoft.AspNetCore.Identity;

namespace EtaAnnouncer.Models
{
    public class Member : IdentityUser
    {
        public List<Home> Homes { get; set; }
    }
}
