using MediatR;
using Microsoft.EntityFrameworkCore;
using VehicleMangement.Commands;
using VehicleMangement.Data;
using VehicleMangement.Events;
using VehicleMangement.EventStore;
using VehicleMangement.Models;

namespace VehicleMangement.Handlers
{
    public class CreateVehicleHandler:IRequestHandler<CreateVehicleCommand,VehicleDetails>
    {
       // private readonly UserDbContext _context;
        private readonly ReadDbContext _readcontext;
        private readonly EventRepository _repository;
        private readonly VehicleProjectionHandler _projection;
        public CreateVehicleHandler(EventRepository repository,ReadDbContext readcontext,VehicleProjectionHandler projection)
        {
           
            _readcontext= readcontext;
            _repository= repository;
            _projection= projection;
        }
        public async Task<VehicleDetails> Handle(CreateVehicleCommand command,CancellationToken ct)
        {
            var exits=await _readcontext.Vehicles.AnyAsync(x=>x.VehicleNumber==command.VehicleNumber,ct);
            if(exits)
            {
                throw new InvalidOperationException("VehicleNumber already exists");
            }
            var today=DateTime.UtcNow.ToString("yyyyMMdd");
            var lastship = await _readcontext.Vehicles.MaxAsync(v => (string?)v.ShipmentNumber, ct);
            int nextseq = lastship == null ? 1 : int.Parse(lastship.Split('-')[2]) + 1;
            var shipmentNumber=$"SHIP-{today}-{nextseq:D3}";
            var vehicle = new VehicleDetails
            {
                VehicleNumber = command.VehicleNumber,
                Type = command.Type,
                ShipmentNumber = shipmentNumber,
                LoadMaterial = command.LoadMaterial,
                DriverName = command.DriverName,
                Source = command.Source,
                Destination = command.Destination,
                CreatedBy = command.CreatedBy
            };
            var evt = new VehicleCreatedEvent
            {
                VehicleId = vehicle.VehicleId,
                VehicleNumber = vehicle.VehicleNumber,
                ShipmentNumber = vehicle.ShipmentNumber,
                VehicleType = vehicle.Type.ToString(),
                LoadMaterial = vehicle.LoadMaterial,
                DriverName = vehicle.DriverName,
                Source = vehicle.Source,
                Destination = vehicle.Destination,
                CreatedBy = vehicle.CreatedBy
            };

            await _repository.SaveAsync(vehicle.VehicleId,evt);
            await _projection.Handle(evt);
            return vehicle;
        }
            
    }
}
