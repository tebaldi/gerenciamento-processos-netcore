using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FT.GerenciamentoProcessosJudiciais.Infra.Mailing
{
    public class EmailSender : IEmailSender
    {
        readonly ILogger<EmailSender> logger;
        readonly EmailSettings settings;

        public EmailSender(ILogger<EmailSender> logger, IOptions<EmailSettings> options)
        {
            this.logger = logger;
            this.settings = options.Value;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            using (var logScope = logger.BeginScope($"SendAsync {nameof(EmailSettings)}: {settings}"))
            {
                logger.LogDebug($"Sending email to {to} with subject {subject}");

                try
                {
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(settings.UserName));
                    message.To.Add(new MailboxAddress(to));
                    message.Subject = subject;
                    message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

                    using (var client = new SmtpClient())
                    {
                        await client.ConnectAsync(host: settings.Host, port: settings.Port, options: settings.UseSsl ?
                            MailKit.Security.SecureSocketOptions.SslOnConnect :
                            MailKit.Security.SecureSocketOptions.None);
                        await client.AuthenticateAsync(userName: settings.UserName, password: settings.Password);
                        await client.SendAsync(message);
                        await client.DisconnectAsync(true);
                    }

                    logger.LogInformation($"Email sent to {to} with subject {subject}");
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, $"SendAsync error to {to} with subject {subject}");
                    throw ex;
                }
            }
        }
    }
}
