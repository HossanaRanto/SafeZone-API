namespace SafeZone.Models
{
    public class PasswordResetCode
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int Code { get; set; }
        public DateTime Validity { get; set; } = DateTime.Now.AddHours(3);
    }
}
