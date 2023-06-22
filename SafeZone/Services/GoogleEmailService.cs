using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Text;
using SafeZone.Repositories;
using SafeZone.Data;

namespace SafeZone.Services
{
    public class GoogleEmailService:IEmailRepository
    {
        private readonly IConfiguration configuration;

        public GoogleEmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task Send(string to, string subject, string body)
        {
            UserCredential credential;

            var path = configuration.GetValue<string>(Variables.CredentialPaht);
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    new[] { GmailService.Scope.GmailSend },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));


            }

            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Safe Zone"
            });

            
            var message = new Message();
            string message_content = $"To: {to}\r\nSubject: {subject}\r\nContent-Type: text/plain;charset=utf-8\r\n\r\n{body}";

            message.Raw = Base64UrlEncode(message_content);

            var send = service.Users.Messages.Send(message, "me");
            await send.ExecuteAsync();
        }
        public string Base64UrlEncode(string input)
        {
            var data = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(data).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}
