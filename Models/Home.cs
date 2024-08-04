namespace EtaAnnouncer.Models
{
    public class Home
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Member> Members { get; set; }
        public List<HomeInvitation> Invitations { get; set; }
    }

    public class HomeInvitation
    {
        public Guid Id { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }
        public Home Home { get; set; }
    }
}
