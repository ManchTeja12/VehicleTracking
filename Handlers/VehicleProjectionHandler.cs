using VehicleMangement.Data;
using VehicleMangement.Enums;
using VehicleMangement.Events;
using VehicleMangement.Models;

namespace VehicleMangement.Handlers
{
    public class VehicleProjectionHandler
    {
        private readonly ReadDbContext _readcontext;

        public VehicleProjectionHandler(ReadDbContext readcontext)
        {
            _readcontext = readcontext;
        }

        public async Task Handle(VehicleCreatedEvent e)
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
                CreatedAt = DateTime.UtcNow
            };

            _readcontext.Vehicles.Add(vehicle);

            await _readcontext.SaveChangesAsync();
        }
    }

}
