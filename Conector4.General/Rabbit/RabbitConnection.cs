using RabbitMQ.Client;

public static class RabbitConnection
{
    public static ConnectionFactory Create()
    {
        return new ConnectionFactory
        {
            HostName = "vms.websamos.com.br",
            Port = 5672,
            VirtualHost = "/",
            UserName = "ti",
            Password = "Ti@2015!*"
        };
    }
}
