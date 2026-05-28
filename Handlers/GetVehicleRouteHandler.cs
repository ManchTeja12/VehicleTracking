using MediatR;
using VehicleMangement.Queries;
using VehicleMangement.Services;

namespace VehicleMangement.Handlers
{
    public class GetVehicleRouteHandler : IRequestHandler<GetVehicleRouteQuery, List<double[]>>
    {
        private readonly IRouteService _routeService;

        public GetVehicleRouteHandler(IRouteService routeService)
        {
            _routeService = routeService;
        }

        public async Task<List<double[]>> Handle(GetVehicleRouteQuery request, CancellationToken ct)
        {
            return await _routeService.GetRoute(request.SourceLat, request.SourceLng, request.DestLat, request.DestLng);
        }
    }
}
