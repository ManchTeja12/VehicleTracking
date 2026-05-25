using System.Text.Json;
using VehicleMangement.Data;
using VehicleMangement.Models;
namespace VehicleMangement.EventStore
{
    public class EventRepository
    {
        private readonly UserDbContext _context;
        public EventRepository(UserDbContext context)
        {
            _context = context;
        }
        public async Task SaveAsync(Guid aggregateId,object @event)
        {
            var Entity = new EventEntity
            {
                AggregateId = aggregateId.ToString(),
                EventType = @event.GetType().Name,
                EventData = JsonDocument.Parse(JsonSerializer.Serialize(@event)),
            };
            _context.Events.Add(Entity);
            await _context.SaveChangesAsync();
        }
    }
}
