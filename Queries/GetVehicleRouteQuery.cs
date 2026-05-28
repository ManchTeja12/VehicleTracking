using MediatR;

namespace VehicleMangement.Queries
{
    public class GetVehicleRouteQuery : IRequest<List<double[]>>
    {
        public double SourceLat { get; set; }
        public double SourceLng { get; set; }
        public double DestLat { get; set; }
        public double DestLng { get; set; }
    }
}
