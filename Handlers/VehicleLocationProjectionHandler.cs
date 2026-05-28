using MediatR;
using Microsoft.AspNetCore.SignalR;
using VehicleMangement.Events;
using VehicleMangement.Hubs;

namespace VehicleMangement.Handlers
{
    public class VehicleLocationProjectionHandler : INotificationHandler<VehicleLocationUpdatedEvent>
    {
        private readonly IHubContext<RouteHub> _hubContext;

        public VehicleLocationProjectionHandler(IHubContext<RouteHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(VehicleLocationUpdatedEvent notification, CancellationToken ct)
        {
            // Broadcast the location update to the specific SignalR group representing this vehicle.
            // Subscribers will receive this event in real-time.
            await _hubContext.Clients.Group(notification.VehicleId.ToString())
                .SendAsync("RouteCoordinate", new
                {
                    vehicleId = notification.VehicleId.ToString(),
                    lat = notification.Latitude,
                    lng = notification.Longitude
                });
        }
    }
}
