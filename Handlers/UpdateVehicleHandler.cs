using MediatR;
using Microsoft.EntityFrameworkCore;
using VehicleMangement.Commands;
using VehicleMangement.Data;
using VehicleMangement.Events;
using VehicleMangement.EventStore;
using VehicleMangement.Models;
using VehicleMangement.Services;

namespace VehicleMangement.Handlers
{
    public class UpdateVehicleHandler : IRequestHandler<UpdateVehicleCommand, VehicleDetails>
    {
        private readonly ReadDbContext _readcontext;
        private readonly EventRepository _repository;
        private readonly VehicleProjectionHandler _projection;
        public UpdateVehicleHandler(ReadDbContext readcontext, EventRepository repository, VehicleProjectionHandler projection  )
        {

            _readcontext = readcontext;
            _repository = repository;
            _projection = projection;
        }
        public async Task<VehicleDetails> Handle(UpdateVehicleCommand command, CancellationToken ct)
        {
            var vehicle = await _readcontext.Vehicles.FirstOrDefaultAsync(x => x.VehicleId == command.VehicleId, ct);
            if (vehicle == null)
            {
                throw new KeyNotFoundException("Vehicle not Found");
            }
            var evt = new VehicleUpdateEvent
            {
                VehicleId = command.VehicleId,

                VehicleNumber = command.VehicleNumber ?? vehicle.VehicleNumber,

                Type = command.Type ?? vehicle.Type,

                LoadMaterial = command.LoadMaterial ?? vehicle.LoadMaterial,

                DriverName = command.DriverName ?? vehicle.DriverName,

                Source = command.Source ?? vehicle.Source,

                Destination = command.Destination ?? vehicle.Destination,

                UpdatedAt = DateTime.UtcNow
            };

            await _repository.SaveAsync(vehicle.VehicleId, evt);
            await _projection.Handle(evt);

           

            return new VehicleDetails
            {
                VehicleId = evt.VehicleId,
                VehicleNumber = evt.VehicleNumber,
                Type = evt.Type.Value,
                LoadMaterial = evt.LoadMaterial,
                DriverName = evt.DriverName,
                Source = evt.Source,
                Destination = evt.Destination,
                ShipmentNumber = vehicle.ShipmentNumber,
                CreatedBy = vehicle.CreatedBy,
                CreatedAt = vehicle.CreatedAt
            };
        }

    }
}
