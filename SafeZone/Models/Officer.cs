namespace SafeZone.Models
{
    public class Officer
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int BadgeNumber { get; set; }
        public OfficerRank Rank { get; set; }
    }
}
