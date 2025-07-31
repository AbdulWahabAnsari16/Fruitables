using System.Net.Mail;
using System.Net;

namespace Fruitables.Services
{
    public class EmailService
    {
        private IConfiguration config;

        public EmailService(IConfiguration config)
        {
            this.config = config;
        }

        public void SendEmail(string Remail, string Subject, string Message)
        {
            var data = config.GetSection("credentials");
            var port = int.Parse(data["port"]);
            var host = data["host"];
            var email = data["email"];
            var password = data["password"];

            SmtpClient smtp = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(email, password),
                EnableSsl = true
            };
            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(email),
                Subject = Subject,
                Body = Message,
            };
            mail.To.Add(new MailAddress(Remail));
            smtp.Send(mail);
        }
    }
}
