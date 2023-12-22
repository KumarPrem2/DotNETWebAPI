namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService
    {
        private readonly string _mailTo = String.Empty;
        private readonly string _mailFrom = String.Empty;

        public CloudMailService(IConfiguration configuration)
        {
            _mailTo = configuration["mailSettings:mailToAddress"];
            _mailFrom = configuration["mailSettings:mailFromAddress"];
        }
        public void send(string subject, string message)
        {
            Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, " +
                $"With name of {nameof(CloudMailService)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
