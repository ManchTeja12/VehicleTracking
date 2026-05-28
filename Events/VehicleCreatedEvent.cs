using MediatR;
using Microsoft.VisualBasic;

namespace VehicleMangement.Events
{
    public class VehicleCreatedEvent : INotification
    {
        public Guid VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string ShipmentNumber { get; set; } = string.Empty;
        public string LoadMaterial { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; }= string.Empty;
        public double SourceLat { get; set; }
        public double SourceLng { get; set; }
        public double DestLat { get; set; }
        public double DestLng { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}

