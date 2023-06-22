namespace SafeZone.Models
{
    public class Crime
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public User Publisher { get; set; }
        public DateTime Created_at { get; set; }=DateTime.Now;
        public string Coordinates { get; set; }
        public Officer Officer { get; set; }
        public bool IsOk { get; set; } = false;
        public int Level { get; set; }
    }
}
