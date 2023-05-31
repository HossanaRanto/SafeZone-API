namespace SafeZone.DTO
{
    public class ResetPasswordDTO
    {
        public string Username { get; set; }
        public int Code { get; set; }
        public string Password { get; set; }
    }
}
