using RabbitMQ.Client;
using System.Text;

namespace Conector4.General.Rabbit
{
    public class RabbitProducer
    {
        public async Task PublishAsync(string queueName, string json)
        {
            var factory = new ConnectionFactory
            {
                HostName = "vms.websamos.com.br",
                Port = 5672,
                VirtualHost = "/",
                UserName = "ti",
                Password = "Ti@2015!*"
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                mandatory: false,
                basicProperties: props,
                body: body
            );
        }
    }
}
