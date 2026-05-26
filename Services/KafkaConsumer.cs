using System.Text.Json;
using VehicleMangement.Events;
using VehicleMangement.Handlers;

namespace VehicleMangement.Services
{
    public class KafkaConsumer
    {
    //    private readonly IServiceScopeFactory _scopeFactory;

    //    public KafkaConsumer(IServiceScopeFactory scopeFactory)
    //    {
    //        _scopeFactory = scopeFactory;
    //    }

    //    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        return Task.Run(() =>
    //        {
    //            var config = new ConsumerConfig
    //            {
    //                BootstrapServers = "localhost:9092",
    //                GroupId = "sync-service",
    //                AutoOffsetReset = AutoOffsetReset.Earliest,
    //                EnableAutoCommit = false
    //            };

    //            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    //            consumer.Subscribe(KafkaTopic.VehicleEvents);

    //            while (!stoppingToken.IsCancellationRequested)
    //            {
    //                try
    //                {
    //                    var result = consumer.Consume(TimeSpan.FromSeconds(1));
    //                    if (result == null) continue;

    //                    var json = result.Message.Value;
    //                    using var scope = _scopeFactory.CreateScope();
    //                    var projection = scope.ServiceProvider.GetRequiredService<VehicleProjectionHandler>();
    //                    var message = JsonDocument.Parse(json);
    //                    var eventType = message.RootElement.GetProperty("EventType").GetString();
    //                    var data = message.RootElement.GetProperty("Data").GetRawText();

    //                    if (eventType == nameof(VehicleCreatedEvent))
    //                    {
    //                        var evt = JsonSerializer.Deserialize<VehicleCreatedEvent>(data);
    //                        projection.Handle(evt).GetAwaiter().GetResult();
    //                    }
    //                    else if (eventType == nameof(VehicleUpdateEvent))
    //                    {
    //                        var evt = JsonSerializer.Deserialize<VehicleUpdateEvent>(data);
    //                        projection.Handle(evt).GetAwaiter().GetResult();
    //                    }

    //                    consumer.Commit(result);
    //                }
    //                catch (Exception ex)
    //                {
    //                    Console.WriteLine($"Kafka consumer error: {ex.Message}");
    //                }
    //            }

    //            consumer.Close();
    //        }, stoppingToken);
    //    }
    }
}
