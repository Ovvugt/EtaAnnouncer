using EtaAnnouncer.Models;

namespace EtaAnnouncer.Contracts
{
    public class HomeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<MemberDto> Members { get; set; }
    }
}
