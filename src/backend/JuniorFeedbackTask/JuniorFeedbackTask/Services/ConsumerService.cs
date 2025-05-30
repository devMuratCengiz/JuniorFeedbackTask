using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using JuniorFeedbackTask.Models;
using MongoDB.Driver;

public class ConsumerService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ConsumerService> _logger;
    private IConnection _connection;
    private IModel _channel;

    public ConsumerService(IServiceScopeFactory scopeFactory, ILogger<ConsumerService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare(queue: "feedback_queue", exclusive: false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, e) =>
        {
            try
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var data = JsonConvert.DeserializeObject<FeedbackDto>(message);

                await ProcessMessageAsync(data);

                _channel.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj işlenirken hata oluştu.");
                _channel.BasicNack(e.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue: "feedback_queue", autoAck: false, consumer: consumer);

        _logger.LogInformation("ConsumerService başlatıldı ve RabbitMQ'ya bağlandı.");
        return Task.CompletedTask;
    }
    private async Task ProcessMessageAsync(FeedbackDto feedback)
    {
        using var scope = _scopeFactory.CreateScope();
        var collection = scope.ServiceProvider.GetRequiredService<IMongoCollection<FeedbackDto>>();

        await collection.InsertOneAsync(feedback);

        _logger.LogInformation("Feedback MongoDB'ye kaydedildi: {@Feedback}", feedback);
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
