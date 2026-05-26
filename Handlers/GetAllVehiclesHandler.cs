using MediatR;
using Microsoft.EntityFrameworkCore;
using VehicleMangement.Data;
using VehicleMangement.Models;
using VehicleMangement.Queries;

namespace VehicleMangement.Handlers
{
    public class GetAllVehiclesHandler : IRequestHandler<GetAllVehiclesQuery, List<VehicleDetails>>
    {
        private readonly ReadDbContext _context;

        public GetAllVehiclesHandler(
            ReadDbContext context)
        {
            _context = context;
        }

        public async Task<List<VehicleDetails>>Handle(GetAllVehiclesQuery request,CancellationToken ct)
        {
            return await _context.Vehicles.AsNoTracking() .ToListAsync(ct);
        }
    
    }
}
