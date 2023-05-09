namespace SafeZone.Models
{
    public class Crime
    {
        public int Id { get; set; }
        public User Publisher { get; set; }
        public DateTime Created_at { get; set; }=DateTime.Now;
        public string Coordinates { get; set; }
    }
}
