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
        //private readonly VehicleProjectionHandler _projection;
        public CreateVehicleHandler(EventRepository repository,ReadDbContext readcontext,VehicleProjectionHandler projection)
        {
           
            _readcontext= readcontext;
            _repository= repository;
            //_projection= projection;
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
            var vehicleId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;

            var vehicleEvent = new VehicleCreatedEvent
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
                CreatedAt = createdAt
            };

            await _repository.SaveAsync(vehicleId, vehicleEvent);

           
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
                CreatedAt = createdAt
            };

            await _repository.SaveAsync(vehicleId,evt);
            //await _projection.Handle(vehicleEvent);

            // This is for kafka

            //await _producer.PublishAsync(KafkaTopic.VehicleEvents, new
            //{
            //    EventType = nameof(VehicleCreatedEvent),
            //    Data = evt
            //});

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
                CreatedAt = createdAt
            };
        }
            
    }
}
