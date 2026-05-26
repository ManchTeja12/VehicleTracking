using VehicleMangement.Enums;

namespace VehicleMangement.Events
{
    public class VehicleUpdateEvent
    {
        public Guid VehicleId { get; set; }
        public string? VehicleNumber { get; set; }
        public VehicleType? Type { get; set; }
        public string? LoadMaterial { get; set; }
        public string? DriverName { get; set; }
        public string? Source { get; set; }
        public string? Destination { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
