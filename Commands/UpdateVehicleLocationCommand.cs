using MediatR;

namespace VehicleMangement.Commands
{
    public class UpdateVehicleLocationCommand : IRequest
    {
        public string VehicleId { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
