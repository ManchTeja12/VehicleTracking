using MediatR;
using VehicleMangement.Enums;
using VehicleMangement.Models;

namespace VehicleMangement.Commands
{
    public class CreateVehicleCommand:IRequest<VehicleDetails>
    {
        public string VehicleNumber { get; set; } = string.Empty;
        public VehicleType Type { get; set; }
        public string LoadMaterial { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;

        public double SourceLat { get; set; }
        public double SourceLng { get; set; }
        public double DestLat { get; set; }
        public double DestLng { get; set; }
        public string Destination { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }
}
