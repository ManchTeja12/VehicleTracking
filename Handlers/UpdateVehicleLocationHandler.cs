using MediatR;
using VehicleMangement.Commands;
using VehicleMangement.Events;
using VehicleMangement.EventStore;

namespace VehicleMangement.Handlers
{
    public class UpdateVehicleLocationHandler : IRequestHandler<UpdateVehicleLocationCommand>
    {
        private readonly EventRepository _repository;
        private readonly IMediator _mediator;

        public UpdateVehicleLocationHandler(EventRepository repository, IMediator mediator)
        {
            _repository = repository;
            _mediator = mediator;
        }

        public async Task Handle(UpdateVehicleLocationCommand command, CancellationToken ct)
        {
            if (!Guid.TryParse(command.VehicleId, out var vehicleGuid))
            {
                throw new ArgumentException("Invalid Vehicle ID");
            }

            var evt = new VehicleLocationUpdatedEvent
            {
                VehicleId = vehicleGuid,
                Latitude = command.Latitude,
                Longitude = command.Longitude,
                Timestamp = DateTime.UtcNow
            };

            // Save location update to the Event Store (Write Model)
            await _repository.SaveAsync(vehicleGuid, evt);

            // Publish event to trigger projections and SignalR broadcasts
            await _mediator.Publish(evt, ct);
        }
    }
}
