namespace SafeZone.Models
{
    public class Notify
    {
        public int Id { get; set; }
        public User Notifer { get; set; }
        public DateTime Notified_at { get; set; }=DateTime.Now;
        public string Coordinates { get; set; }
      
    }
}
