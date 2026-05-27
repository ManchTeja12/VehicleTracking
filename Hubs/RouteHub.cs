using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VehicleMangement.Data;
using VehicleMangement.Services;

namespace VehicleMangement.Hubs
{
    public class RouteHub : Hub
    {
        private readonly IRouteService _routeService;
        private readonly ReadDbContext _context;

        public RouteHub(IRouteService routeService,ReadDbContext context)
        {
            _routeService = routeService;
            _context = context;
        }

        // Runs automatically when client connects
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client Connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        // Runs automatically when client disconnects
        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            Console.WriteLine($"Client Disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(ex);
        }

        // Join a vehicle group
        public async Task JoinVehicle(string vehicleId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId,vehicleId);
            Console.WriteLine($"{Context.ConnectionId} joined {vehicleId}");
            await Clients.Caller.SendAsync("Joined",vehicleId);
        }

        // Leave a vehicle group
        public async Task UnsubscribeVehicle(string vehicleId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId,vehicleId);
            Console.WriteLine($"{Context.ConnectionId} left {vehicleId}");
            await Clients.Caller.SendAsync("Unsubscribed",vehicleId);
        }
        public async Task StartTrip(string vehicleId)
        {
            try
            {
                // safer than Guid.Parse()
                if (!Guid.TryParse(vehicleId, out var id))
                {
                    await Clients.Caller.SendAsync("Error","Invalid Vehicle Id");
                    return;
                }

                var vehicle =await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == id);
                if (vehicle == null)
                {
                    await Clients.Caller.SendAsync("Error","Vehicle Not Found");
                    return;
                }

                var coordinates =await _routeService.GetRoute(
                        vehicle.SourceLat,
                        vehicle.SourceLng,
                        vehicle.DestLat,
                        vehicle.DestLng);

                if (coordinates == null ||coordinates.Count == 0)
                {
                    await Clients.Caller.SendAsync("Error","No route found");
                    return;
                }

                await Clients.Group(vehicleId).SendAsync("RouteStart",coordinates.Count);

                for (int i = 0; i < coordinates.Count; i += 3)
                {
                    var point = coordinates[i];

                    Console.WriteLine($"Lat:{point[1]} Lng:{point[0]}");

                    await Clients.Group(vehicleId)
                    .SendAsync(
                        "RouteCoordinate",
                        new
                        {
                            lat = point[1],
                            lng = point[0]
                        });

                    await Task.Delay(50);
                }

                await Clients.Group(vehicleId).SendAsync("RouteEnd");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                await Clients.Caller.SendAsync("Error", ex.Message);


            }
        }
    }
}