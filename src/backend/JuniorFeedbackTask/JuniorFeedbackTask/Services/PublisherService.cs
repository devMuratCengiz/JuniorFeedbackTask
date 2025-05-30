using JuniorFeedbackTask.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

public class PublisherService : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public PublisherService()
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "feedback_queue", exclusive: false);
    }

    public void SendToQueue(FeedbackDto model)
    {
        var message = JsonConvert.SerializeObject(model);
        var body = Encoding.UTF8.GetBytes(message);

        _channel.BasicPublish(exchange: "", routingKey: "feedback_queue", body: body);
    }

    public void Dispose()
    {
        _channel?.Close();
        _channel?.Dispose();

        _connection?.Close();
        _connection?.Dispose();
    }
}
