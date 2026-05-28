
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VehicleMangement.Commands;
using VehicleMangement.Queries;
using VehicleMangement.Services;

namespace VehicleMangement.Hubs
{
    public class RouteHub : Hub
    {

        private readonly IMediator _mediator;
        private readonly SubscriptionService _subscriptionService;

        public RouteHub(IMediator mediator, SubscriptionService subscriptionService)
        {
            _mediator = mediator;
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

        // start trip stimulation
        public async Task StartTrip(string vehicleId)
        {
            var connectionId = Context.ConnectionId; // every client has unique connection id 

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
            var vehicle = await _mediator.Send(new GetVehicleByIdQuery { VehicleId = id });
            if (vehicle == null)
            {
                await Clients.Caller.SendAsync("Error", "Vehicle not found");
                return;
            }

            Console.WriteLine($"VehicleId : {vehicle.VehicleId}");
            Console.WriteLine($"Source : {vehicle.SourceLat},{vehicle.SourceLng}");
            Console.WriteLine($"Destination : {vehicle.DestLat},{vehicle.DestLng}");

            var coordinates = await _mediator.Send(new GetVehicleRouteQuery
            {
                SourceLat = vehicle.SourceLat,
                SourceLng = vehicle.SourceLng,
                DestLat = vehicle.DestLat,
                DestLng = vehicle.DestLng
            });

            if (coordinates == null || coordinates.Count == 0)
            {
                await Clients.Caller.SendAsync("Error", "No route found");
                return;
            }

            // Capture caller before Task.Run
            var caller = Clients.Caller; // storing refernce to current client

            // Run loop in background — frees hub for other calls
            _ =Task.Run(async () => // _=this symbol tells idont want for result 
            {
                try
                {
                    await caller.SendAsync("RouteStart", coordinates.Count);
                    foreach (var point in coordinates)
                    {
                        // Stop if vehicle switched
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
                            lng = point[0] // because osrm gives lng and lat so lng=0,lat=1
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

        // send coordinate (allows client to report location update)
        public async Task SendCoordinate(string vehicleId, double lat, double lng)
        {
            try
            {
                var connectionId = Context.ConnectionId;
                if (!_subscriptionService.IsSubscribed(connectionId, vehicleId))
                {
                    await Clients.Caller.SendAsync("Error", "You must subscribe first");
                    return;
                }

                await Clients.Group(vehicleId).SendAsync("RouteCoordinate", new
                {
                    vehicleId = vehicleId,
                    lat = lat,
                    lng = lng
                 });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendCoordinate error: {ex.Message}");
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

    }
}
