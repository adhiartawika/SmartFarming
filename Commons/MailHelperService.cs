using backend.Controllers;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using NETCore.MailKit.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Commons
{
    public interface IMailHelperService
    {
        void SendMail(string to, string subject, string content);
    }
    public class MailHelperService : IMailHelperService
    {
        IConfiguration configuration;

        public MailHelperService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        public void SendMail(string to, string subject, string content)
        {
            try
            {
                var mailKitOptions = configuration.GetSection("EmailSettings").Get<MailKitOptions>();

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(mailKitOptions.SenderEmail, mailKitOptions.SenderEmail));
                message.To.Add(new MailboxAddress(to, to));
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = content
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(mailKitOptions.Server, mailKitOptions.Port,mailKitOptions.Security);
                    client.Authenticate(mailKitOptions.Account, mailKitOptions.Password);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
