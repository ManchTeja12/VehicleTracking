using MediatR;
using VehicleMangement.Models;

namespace VehicleMangement.Queries
{
    public class GetVehicleByIdQuery:IRequest<VehicleDetails>
    {
        public Guid VehicleId {  get; set; }
    }
}
