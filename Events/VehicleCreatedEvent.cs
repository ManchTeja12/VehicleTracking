namespace VehicleMangement.Events
{
    public class VehicleCreatedEvent
    {
        public Guid VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string ShipmentNumber { get; set; } = string.Empty;
        public string LoadMaterial { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; }= string.Empty;
        public string CreatedBy { get; set; } = string.Empty;

    }
}
