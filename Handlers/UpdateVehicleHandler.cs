using MediatR;
using Microsoft.EntityFrameworkCore;
using VehicleMangement.Commands;
using VehicleMangement.Data;
using VehicleMangement.Events;
using VehicleMangement.EventStore;
using VehicleMangement.Models;

namespace VehicleMangement.Handlers
{
    public class UpdateVehicleHandler : IRequestHandler<UpdateVehicleCommand, VehicleDetails>
    {
        private readonly UserDbContext _context;
        private readonly ReadDbContext _readcontext;
        private readonly EventRepository _repository;
        public UpdateVehicleHandler(UserDbContext context,ReadDbContext readcontext, EventRepository repository)
        {
            _context = context;
            _readcontext = readcontext;
            _repository = repository;
        }
        public async Task<VehicleDetails> Handle(UpdateVehicleCommand command,CancellationToken ct)
        {
            var vehicle = await _readcontext.Vehicles.FirstOrDefaultAsync(x => x.VehicleId == command.VehicleId, ct);
            if(vehicle==null)
            {
                throw new KeyNotFoundException("Vehicle not Found");
            }
            if (command.VehicleNumber != null)
                vehicle.VehicleNumber = command.VehicleNumber;

            if (command.Type != null)
                vehicle.Type = command.Type.Value;

            if (command.LoadMaterial != null)
                vehicle.LoadMaterial = command.LoadMaterial;

            if (command.DriverName != null)
                vehicle.DriverName = command.DriverName;

            if (command.Source != null)
                vehicle.Source = command.Source;

            if (command.Destination != null)
                vehicle.Destination = command.Destination;

            await _repository.SaveAsync(vehicle.VehicleId, new VehicleUpdateEvent
            {
                VehicleId = vehicle.VehicleId,
                VehicleNumber = vehicle.VehicleNumber,
                Type = vehicle.Type,
                LoadMaterial = vehicle.LoadMaterial,
                DriverName = vehicle.DriverName,
                Source = vehicle.Source,
                Destination = vehicle.Destination,
                UpdatedAt = DateTime.UtcNow
            });
            return vehicle;

        }
    }
}
