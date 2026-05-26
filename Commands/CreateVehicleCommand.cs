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
        public string Destination { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }
}
