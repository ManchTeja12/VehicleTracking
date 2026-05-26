using MediatR;
using Microsoft.EntityFrameworkCore;
using VehicleMangement.Data;
using VehicleMangement.Models;
using VehicleMangement.Queries;

namespace VehicleMangement.Handlers
{
    public class GetVehicleByIdHandler :
        IRequestHandler<GetVehicleByIdQuery, VehicleDetails?>
    {
        private readonly ReadDbContext _context;

        public GetVehicleByIdHandler(
            ReadDbContext context)
        {
            _context = context;
        }

        public async Task<VehicleDetails?> Handle(
            GetVehicleByIdQuery request,
            CancellationToken ct)
        {
            return await _context.Vehicles
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.VehicleId == request.VehicleId,
                    ct);
        }
    }
}
