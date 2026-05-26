using System.Text.Json;

namespace VehicleMangement.Services
{
    public class KafkaProducer
    {
        //private readonly IProducer<Null, string> _producer; //key value pair
        //public KafkaProducer() //constructor to initialize the Kafka producer with the necessary configuration
        //{
        //    var config = new ProducerConfig
        //    {
        //        BootstrapServers = "localhost:9092"//address of the Kafka broker
        //    };
        //    _producer = new ProducerBuilder<Null, string>(config).Build(); //this creates  builder object 
        //                                                                   //.Build() method creates the producer object 
        //}

        //public async Task PublishAsync(string topic, object data)
        //{
        //    var json = JsonSerializer.Serialize(data);//c# objct to json string
        //    await _producer.ProduceAsync(topic, new Message<Null, string>
        //    {
        //        Value = json
        //    });

        //}
    }
}
