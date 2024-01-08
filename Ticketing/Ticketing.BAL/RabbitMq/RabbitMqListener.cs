using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Microsoft.Extensions.Hosting;
using System.Text;
using Ticketing.BAL.Contracts;
using System.Text.Json;
using Ticketing.DAL.Domains;
using Microsoft.Extensions.Options;
using Ticketing.BAL.Options;
using log4net;

namespace Ticketing.BAL.RabbitMq
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IEmailService _emailService;
        private readonly IOptions<RabbitMqSettings> _options;
        private readonly ILog _logger;

        public RabbitMqListener(IEmailService emailService, IOptions<RabbitMqSettings> options, ILog logger)
        {
            _emailService = emailService;
            _options = options;
            _logger = logger;

            var factory = new ConnectionFactory { HostName = _options.Value.HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _options.Value.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());

                _logger.Debug("RabbitMqListener content { content}");

                var email = JsonSerializer.Deserialize<EmailNotification>(content, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                _channel.BasicAck(ea.DeliveryTag, false);
                if (email is not null)
                {
                    await _emailService.SendEmailAsync(email);
                }
            };

            _channel.BasicConsume(_options.Value.QueueName, false, consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            GC.SuppressFinalize(this);
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
