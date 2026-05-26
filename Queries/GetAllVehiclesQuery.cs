using MediatR;
using VehicleMangement.Models;

namespace VehicleMangement.Queries
{
    public class GetAllVehiclesQuery:IRequest<List<VehicleDetails>>
    { 
    }
}
