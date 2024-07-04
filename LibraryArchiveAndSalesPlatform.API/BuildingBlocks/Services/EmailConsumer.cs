using System.Text;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MailKit.Net.Smtp;
using LibraryArchiveAndSalesPlatform.API.BuildingBlocks.IServices;
using Microsoft.AspNetCore.Identity;
using LibraryArchiveAndSalesPlatform.API.Domain.Models;

namespace LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Services
{
    public class EmailConsumer : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EmailSettings _emailSettings;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public EmailConsumer
            (
                IOptions<EmailSettings> emailSettings, 
                IUserService userService, 
                UserManager<User> userManager
            )
        {
            _emailSettings = emailSettings.Value;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "emailQueue",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
            _userService = userService;
            _userManager = userManager;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var emailMessage = JsonConvert.DeserializeObject<EmailMessage>(message);
                await SendEmailAsync(emailMessage);
            };
            _channel.BasicConsume(queue: "emailQueue",
                                  autoAck: true,
                                  consumer: consumer);
            return Task.CompletedTask;
        }

        private async Task SendEmailAsync(EmailMessage emailMessage)
        {
            var currentUser = await _userManager.FindByNameAsync(_userService.GetUserName());
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(currentUser.UserName, currentUser.Email));
            email.To.Add(new MailboxAddress("", emailMessage.ToEmail));
            email.Subject = emailMessage.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = emailMessage.Body };
            email.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, false);
            await client.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);
        }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
