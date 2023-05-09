namespace SafeZone.Repositories
{
    public interface IEmailRepository
    {
        Task Send(string to, string subject, string body);
        string Base64UrlEncode(string input);
    }
}
