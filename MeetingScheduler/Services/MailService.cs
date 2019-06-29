using MailKit.Net.Smtp;
using MeetingScheduler.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingScheduler.Services
{
    public class MailSettings
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SSL { get; set; }
    }
    public class MailService
    {
        private readonly string _server;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly bool _ssl;
        

        public MailService(IOptions<MailSettings> setemail)
        {
            _server = setemail.Value.Server;
            _port = Convert.ToInt32(setemail.Value.Port);
            _username = setemail.Value.UserName;
            _password = setemail.Value.Password;
            _ssl = Convert.ToBoolean(setemail.Value.SSL);
        }
        public async Task SendEmailAsync(string[] emails, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Meet", _username));
            foreach (var email in emails)
            {
                emailMessage.To.Add(new MailboxAddress("", email));
            }
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_server, _port, _ssl);
                    await client.AuthenticateAsync(_username, _password);
                    await client.SendAsync(emailMessage);
                }
                catch (Exception e)
                {

                    Console.WriteLine("Error sent message {0}", e.Message);
                }
                finally
                {

                    await client.DisconnectAsync(true);
                }
            }
        }

        public async Task SendEmailAsync(Message message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Meet", _username));
            foreach (var email in message.Users)
            {
                emailMessage.To.Add(new MailboxAddress("", email));
            }
            emailMessage.Subject = message.Theme;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message.Body
            };

            await Send(emailMessage);
        }
        private async Task Send(MimeMessage emailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_server, _port, _ssl);
                    await client.AuthenticateAsync(_username, _password);
                    await client.SendAsync(emailMessage);
                }
                catch (Exception e)
                {

                    Console.WriteLine("Error sent message {0}", e.Message);
                }
                finally
                {

                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
