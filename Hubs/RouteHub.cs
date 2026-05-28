
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
        private readonly SubscriptionService _subscriptionService;

        public RouteHub(IRouteService routeService,ReadDbContext context, SubscriptionService subscriptionService)
        {
            _routeService = routeService;
            _context = context;
            _subscriptionService = subscriptionService;
        }

        // client connected
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client Connected : {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        // client disconnected
        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            Console.WriteLine($"Client Disconnected : {Context.ConnectionId}");
            var oldVehicle =_subscriptionService.GetVehicle(Context.ConnectionId);

            if (oldVehicle != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId,oldVehicle);
            }

            _subscriptionService.RemoveConnection( Context.ConnectionId);

            await base.OnDisconnectedAsync(ex);
        }

        // subscribe vehicle
        public async Task SubscribeVehicle(string vehicleId)
        {

            try
            {
                Console.WriteLine($"Subscribe Request : {vehicleId}");

                // remove old group if exists
                var oldVehicle =_subscriptionService.GetVehicle(Context.ConnectionId);

                if (oldVehicle != null)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId,oldVehicle);
                    Console.WriteLine($"Removed From Old Group : {oldVehicle}");
                }

                // save new subscription
                _subscriptionService.Subscribe(Context.ConnectionId,vehicleId);

                // add to signalr group
                await Groups.AddToGroupAsync(Context.ConnectionId,vehicleId);

                Console.WriteLine($"{Context.ConnectionId} Joined {vehicleId}");

                await Clients.Caller.SendAsync("Joined",vehicleId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Clients.Caller.SendAsync("Error",ex.Message);
            }
        }

        // unsubscribe
        public async Task UnsubscribeVehicle(string vehicleId)
        {
            try
            {
                _subscriptionService.Unsubscribe(Context.ConnectionId,vehicleId);

                await Groups.RemoveFromGroupAsync(Context.ConnectionId,vehicleId);

                Console.WriteLine($"{Context.ConnectionId} Left {vehicleId}");

                await Clients.Caller.SendAsync("Unsubscribed",vehicleId);
                Console.WriteLine($"Unsubscribed{vehicleId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Clients.Caller.SendAsync("Error",ex.Message);
            }
        }

        // start trip
        public async Task StartTrip(string vehicleId)

        {

            var connectionId = Context.ConnectionId;

            Console.WriteLine($"Current Connection : {connectionId}");

            Console.WriteLine($"Requested Vehicle : {vehicleId}");

            Console.WriteLine($"Subscribed Vehicle : {_subscriptionService.GetVehicle(connectionId)}");

            if (!_subscriptionService.IsSubscribed(connectionId, vehicleId))

            {
                await Clients.Caller.SendAsync("Error", "You must subscribe first");
                return;
            }
            if (!Guid.TryParse(vehicleId, out var id))
            {
                await Clients.Caller.SendAsync("Error", "Invalid Vehicle Id");
                return;
            }
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.VehicleId == id);
            if (vehicle == null)
            {
                await Clients.Caller.SendAsync("Error", "Vehicle not found");
                return;
            }

            Console.WriteLine($"VehicleId : {vehicle.VehicleId}");
            Console.WriteLine($"Source : {vehicle.SourceLat},{vehicle.SourceLng}");
            Console.WriteLine($"Destination : {vehicle.DestLat},{vehicle.DestLng}");

            var coordinates = await _routeService.GetRoute(

                vehicle.SourceLat,

                vehicle.SourceLng,

                vehicle.DestLat,

                vehicle.DestLng);

            if (coordinates == null || coordinates.Count == 0)
            {
                await Clients.Caller.SendAsync("Error", "No route found");

                return;

            }

            // ✅ Capture caller before Task.Run

            var caller = Clients.Caller;

            // ✅ Run loop in background — frees hub for other calls

            _ = Task.Run(async () =>

            {

                try

                {

                    await caller.SendAsync("RouteStart", coordinates.Count);

                    foreach (var point in coordinates)

                    {

                        // ✅ Stop if vehicle switched

                        if (_subscriptionService.GetVehicle(connectionId) != vehicleId)

                        {

                            Console.WriteLine($"Vehicle switched, stopping trip for : {vehicleId}");

                            break;

                        }

                        Console.WriteLine($"Lat : {point[1]} Lng : {point[0]}");

                        await caller.SendAsync("RouteCoordinate", new

                        {

                            vehicleId = vehicleId,

                            lat = point[1],

                            lng = point[0]

                        });

                        await Task.Delay(2000);

                    }

                    if (_subscriptionService.GetVehicle(connectionId) == vehicleId)

                    {

                        await caller.SendAsync("RouteEnd");

                    }

                }

                catch (Exception ex)

                {

                    Console.WriteLine($"Trip error : {ex.Message}");

                }

            });

        }

    }
}
