namespace SafeZone.Models
{
    public class CloseCase
    {
        public int Id { get; set; }
        public Crime Crime { get; set; }
        public string Description { get; set; }
        public DateTime Closed_at { get; set; }=DateTime.Now;
        
    }
}
