using Microsoft.AspNetCore.SignalR;
using VehicleMangement.Services;

namespace VehicleMangement.Hubs
{
    public class RouteHub: Hub
    {
        private readonly IRouteService _routeservice;
        public RouteHub(IRouteService routeservice)
        {
            _routeservice = routeservice;
        }
        public async Task JoinVehicle(string vehicleId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, vehicleId);
        }
        public async Task GetRoute(string vehicleId,double sourceLat, double sourceLong,double destLat,double destLong)
        {
            var coordinates = await _routeservice.GetRoute(sourceLat, sourceLong, destLat, destLong);
            await Clients.Group(vehicleId).SendAsync("Recieve Routes", coordinates);
        }
    }
}
