using MediatR;

namespace VehicleMangement.Events
{
    public class VehicleLocationUpdatedEvent : INotification
    {
        public Guid VehicleId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
