using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using NPOI.OpenXmlFormats.Wordprocessing;
using System.Net;

namespace BackendHrdAgro.Services.Email
{
    //public class MailService : IMailService
    public class MailService : IMailService
    {

        public readonly MailSettings _conf;
        private readonly ILogger<MailService> _logger;

        public MailService(IOptions<MailSettings> configuration, ILogger<MailService> logger)
        {
            _conf = configuration.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(Mailer mailer)
        {
            _logger.LogInformation(message: "Mailer Start");
            var email = new MimeMessage();
            if (_conf.Mail != null && _conf.Mail.Length != 0)
            {
                email.Sender = MailboxAddress.Parse(_conf.Mail);
                email.From.Add(MailboxAddress.Parse(_conf.Mail));
            }
            email.To.Add(MailboxAddress.Parse(mailer.ToEmail));
            List<InternetAddress> ccDddresses = new List<InternetAddress>();
            if (mailer.CC != null)
            {
                foreach (var i in mailer.CC)
                {
                    ccDddresses.Add(MailboxAddress.Parse(i));
                }
                email.Cc.AddRange(ccDddresses);
            }
            email.Subject = mailer.Subject;
            var builder = new BodyBuilder();
            Console.WriteLine("builder mail");
            if (mailer.Attachment != null)
            {
                foreach (var file in mailer.Attachment)
                {
                    if (file != null)
                    {
                        var fileLocation = Path.Combine(mailer.directoryLocation!, file.FileName);
                        Console.WriteLine("file name" + file.Name);
                        builder.Attachments.Add(fileLocation);
                    }
                }
            }
            else
            {
                Console.WriteLine("file is null");
            }
            builder.HtmlBody = mailer.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            try
            {

                smtp.Timeout = 600000;

                //check the port availability 
                if (_conf.Port == null)
                {
                    //connect to smtp server without port
                    smtp.Connect(host: _conf.Host);

                }
                else
                {
                    //connect to smtp server with port
                    smtp.Connect(_conf.Host, _conf.Port.Value, MailKit.Security.SecureSocketOptions.Auto);
                }
                _logger.LogInformation(message: "Connect to SMTP Server");

                if (_conf.IsPlain)
                {
                    smtp.AuthenticationMechanisms.Remove("XOAUTH2");
                    smtp.AuthenticationMechanisms.Add("PLAIN");
                    _logger.LogInformation(message: "No authentication required");
                }
                else
                {
                    if (_conf.Mail != null && _conf.Mail.Length != 0)
                    {
                        if (_conf.Password != null && _conf.Password.Length != 0)
                        {
                            smtp.Authenticate(credentials: new NetworkCredential(userName: _conf.Mail, password: _conf.Password));
                            _logger.LogInformation(message: "Authenticated with user and password");
                        }
                        else
                        {
                            _logger.LogInformation(message: "Authenticated without user and password");
                        }
                    }
                }


                _logger.LogInformation(message: "Send Mail");
                await smtp.SendAsync(email);
                _logger.LogInformation(message: "Email sent");
                smtp.Disconnect(true);
                smtp.Dispose();
                _logger.LogInformation(message: "Email disposed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                _logger.LogError(message: e.Message);
            }
        }

    }
}
