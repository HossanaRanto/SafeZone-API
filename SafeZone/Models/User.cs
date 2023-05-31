namespace SafeZone.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public char Gender { get; set; }
        public DateTime Birthdate { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Coordinates { get; set; }
        public DateTime Created_At { get; set; }=DateTime.Now;
        public bool IsOfficer { get; set; } = false;
        public bool IsVerfied { get; set; } = false;
        public bool IsBlocked { get; set; } = false;
       
    }
}
