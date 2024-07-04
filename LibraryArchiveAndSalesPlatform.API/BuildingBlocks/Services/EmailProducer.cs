using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace LibraryArchiveAndSalesPlatform.API.BuildingBlocks.Services
{
    public class EmailProducer
    {
        private readonly IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel;

        public EmailProducer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "emailQueue",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
        }

        public void PublishEmailRequest(EmailMessage emailMessage)
        {
            var messageBody = JsonConvert.SerializeObject(emailMessage);
            var body = Encoding.UTF8.GetBytes(messageBody);
            _channel.BasicPublish(exchange: "",
                                  routingKey: "emailQueue",
                                  basicProperties: null,
                                  body: body);
        }
    }

    public class EmailMessage
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
