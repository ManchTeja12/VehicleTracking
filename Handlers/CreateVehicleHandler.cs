using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using VehicleMangement.Commands;
using VehicleMangement.Data;
using VehicleMangement.Events;
using VehicleMangement.EventStore;
using VehicleMangement.Models;
using VehicleMangement.Services;

namespace VehicleMangement.Handlers
{
    public class CreateVehicleHandler:IRequestHandler<CreateVehicleCommand,VehicleDetails>
    {
        private readonly ReadDbContext _readcontext;
        private readonly EventRepository _repository;
        private readonly IMediator _mediator;
        public CreateVehicleHandler(EventRepository repository,ReadDbContext readcontext,IMediator mediator)
        {
           
            _readcontext= readcontext;
            _repository= repository;
            _mediator= mediator;
        }
        public async Task<VehicleDetails> Handle(CreateVehicleCommand command,CancellationToken ct)
        {

            if (string.IsNullOrWhiteSpace(command.DriverName))
            {
                throw new Exception("Driver Name is required");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(
                command.DriverName,
                @"^[a-zA-Z\s]+$"))
            {
                throw new Exception("Driver Name should contain only letters");
            }

            if (command.DriverName.Length < 3)
            {
                throw new Exception("Driver Name should be at least 3 characters");
            }

            if (string.IsNullOrWhiteSpace(command.LoadMaterial))
            {
                throw new Exception("Load Material is required");
            }

            if (command.LoadMaterial.Length < 3)
            {
                throw new Exception("Load Material should be at least 3 characters");
            }
            var exits=await _readcontext.Vehicles.AnyAsync(x=>x.VehicleNumber==command.VehicleNumber,ct);
            if(exits)
            {
                throw new InvalidOperationException("VehicleNumber already exists");
            }
            var today=DateTime.UtcNow.ToString("yyyyMMdd");
            var lastship = await _readcontext.Vehicles.Where(v => v.ShipmentNumber.StartsWith($"SHIP-{today}-")).MaxAsync(v => (string?)v.ShipmentNumber, ct);
            int nextseq = lastship == null ? 1 : int.Parse(lastship.Split('-')[2]) + 1;
            var shipmentNumber=$"SHIP-{today}-{nextseq:D3}";
            var vehicleId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;

           
            var evt = new VehicleCreatedEvent
            {
                VehicleId = vehicleId,
                VehicleNumber = command.VehicleNumber,
                ShipmentNumber = shipmentNumber,
                VehicleType = command.Type.ToString(),
                LoadMaterial = command.LoadMaterial,
                DriverName = command.DriverName,
                Source = command.Source,
                Destination = command.Destination,
                CreatedBy = command.CreatedBy,
                CreatedAt = createdAt,

                SourceLat = command.SourceLat,
                SourceLng = command.SourceLng,
                DestLat = command.DestLat,
                DestLng = command.DestLng
            };

            await _repository.SaveAsync(vehicleId,evt);
            await _mediator.Publish(evt, ct);

        
            return new VehicleDetails
            {
                VehicleId = vehicleId,
                VehicleNumber = evt.VehicleNumber,
                ShipmentNumber = evt.ShipmentNumber,
                Type = command.Type,
                LoadMaterial = evt.LoadMaterial,
                DriverName = evt.DriverName,
                Source = evt.Source,
                Destination = evt.Destination,
                CreatedBy = evt.CreatedBy,
                CreatedAt = createdAt,

                SourceLat = command.SourceLat,
                SourceLng = command.SourceLng,
                DestLat = command.DestLat,
                DestLng = command.DestLng
            };
        }
            
    }
}
