namespace SafeZone.Models
{
    public class VerificationCode
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string Code { get; set; }
    }
}
