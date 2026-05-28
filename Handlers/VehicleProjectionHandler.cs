using MediatR;
using Microsoft.EntityFrameworkCore;
using VehicleMangement.Data;
using VehicleMangement.Enums;
using VehicleMangement.Events;
using VehicleMangement.Models;

namespace VehicleMangement.Handlers
{
    public class VehicleProjectionHandler : 
        INotificationHandler<VehicleCreatedEvent>, 
        INotificationHandler<VehicleUpdateEvent>
    {
        private readonly ReadDbContext _readcontext;

        public VehicleProjectionHandler(ReadDbContext readcontext)
        {
            _readcontext = readcontext;
        }

        public async Task Handle(VehicleCreatedEvent e, CancellationToken cancellationToken)
        {
            var vehicle = new VehicleDetails
            {
                VehicleId = e.VehicleId,
                VehicleNumber = e.VehicleNumber,
                ShipmentNumber = e.ShipmentNumber,
                Type = Enum.Parse<VehicleType>(e.VehicleType),
                LoadMaterial = e.LoadMaterial,
                DriverName = e.DriverName,
                Source = e.Source,
                Destination = e.Destination,
                CreatedBy = e.CreatedBy,
                CreatedAt = e.CreatedAt,

                 SourceLat = e.SourceLat,
                SourceLng = e.SourceLng,
                DestLat = e.DestLat,
                DestLng = e.DestLng
            };

            _readcontext.Vehicles.Add(vehicle);

            await _readcontext.SaveChangesAsync();
        }

        public async Task Handle(VehicleUpdateEvent e, CancellationToken cancellationToken)
        {
            var vehicle = await _readcontext.Vehicles.FirstOrDefaultAsync(x => x.VehicleId == e.VehicleId);

            if (vehicle == null)
                return;

            vehicle.VehicleNumber = e.VehicleNumber;

            vehicle.LoadMaterial = e.LoadMaterial;

            vehicle.DriverName = e.DriverName;
            if (e.Type.HasValue)
                vehicle.Type = e.Type.Value;
            vehicle.Source = e.Source;

            vehicle.Destination = e.Destination;

            // overwrite CreatedAt with latest update time
            vehicle.CreatedAt = e.UpdatedAt;

            await _readcontext.SaveChangesAsync();
        }


    }

}
