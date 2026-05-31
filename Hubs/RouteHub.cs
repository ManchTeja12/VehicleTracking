
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using VehicleMangement.Commands;
using VehicleMangement.Queries;
using VehicleMangement.Services;

namespace VehicleMangement.Hubs
{
    public class RouteHub : Hub
    {

        private readonly IMediator _mediator;
        private readonly SubscriptionService _subscriptionService;
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _tripTokens = new();

        public RouteHub(IMediator mediator, SubscriptionService subscriptionService)
        {
            _mediator = mediator;
            _subscriptionService = subscriptionService;
        }

        // client connected
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client Connected : {Context.ConnectionId}");
            await base.OnConnectedAsync(); // base refers to parent class 
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
            if (_tripTokens.TryRemove(Context.ConnectionId, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
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
            if (_tripTokens.TryRemove(connectionId, out var existingCts))
            {
                existingCts.Cancel();
                existingCts.Dispose();
            }

            // ── NEW: create token for this trip ──
            var cts = new CancellationTokenSource();
            _tripTokens[connectionId] = cts;
            var token = cts.Token;

            // Capture caller before Task.Run
            var caller = Clients.Caller; // storing refernce to current client

            // Run loop in background — frees hub for other calls
            _ =Task.Run(async () => // _=this symbol tells idont want for result 
            {
                try
                {
                    await caller.SendAsync("RouteStart", coordinates.Count,token);

                    foreach (var point in coordinates)
                    {
                        token.ThrowIfCancellationRequested(); // ← replaces your if/break check

                        await caller.SendAsync("RouteCoordinate", new
                        {
                            vehicleId = vehicleId,
                            lat = point[1],
                            lng = point[0]
                        }, token); // ← add token

                        await Task.Delay(2000, token); // ← add token (cancels mid-wait too)
                    }

                    await caller.SendAsync("RouteEnd", cancellationToken: token); // ← add token
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"Trip cancelled for: {vehicleId}"); // ← replaces your vehicle check
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Trip error: {ex.Message}");
                }
                finally
                {
                    // ── NEW: clean up the token after trip ends ──
                    if (_tripTokens.TryRemove(connectionId, out var used))
                        used.Dispose();
                }
            }, token);
        }

        public async Task SwitchVehicle(string newVehicleId)
        {
            var connectionId = Context.ConnectionId;
            var oldVehicle =_subscriptionService.GetVehicle(connectionId);

            if (!string.IsNullOrEmpty(oldVehicle))
            {
                await Groups.RemoveFromGroupAsync(connectionId,oldVehicle);
            }

            _subscriptionService.Subscribe(connectionId, newVehicleId);
            await Groups.AddToGroupAsync(connectionId, newVehicleId);
            await StartTrip(newVehicleId);
        }

        // send coordinate (allows client to report location update)
        public async Task SendCoordinate(string vehicleId, double lat, double lng)
        {
            Console.WriteLine($"Sending Current Coordinate{lat}-{lng}");
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
