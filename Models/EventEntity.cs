using System.Text.Json;

namespace VehicleMangement.Models
{
    public class EventEntity
    {
        public int Id { get; set; }
        public string AggregateId { get; set; }= string.Empty;
        public string EventType { get; set; }= string.Empty;
        public JsonDocument EventData { get; set; }=JsonDocument.Parse("{}");
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
