using log4net;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Ticketing.BAL.Contracts;
using Ticketing.BAL.Options;

namespace Ticketing.BAL.Services
{
    public class RabbitMqService : IMessageQueue
    {
        private readonly IOptions<RabbitMqSettings> _options;
        private readonly ILog _logger;
        public RabbitMqService(IOptions<RabbitMqSettings> options, ILog logger)
        {
            _options = options;
            _logger = logger;
        }

        public void SendMessage(object obj)
        {
            var message = JsonSerializer.Serialize(obj);
            SendMessage(message);
        }

        public void SendMessage(string message)
        {
            _logger.Debug($"RabbitMqService Send Message to Queue {_options.Value.QueueName}");

            var factory = new ConnectionFactory { HostName = _options.Value.HostName };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
                channel.QueueDeclare(queue: _options.Value.QueueName,
                               durable: false,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                               routingKey: _options.Value.QueueName,
                               basicProperties: null,
                               body: body);
        }
    }
}
