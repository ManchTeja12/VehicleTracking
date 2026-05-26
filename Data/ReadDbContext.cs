using Microsoft.EntityFrameworkCore;
using VehicleMangement.Models;

namespace VehicleMangement.Data
{
    public class ReadDbContext: DbContext
    {
        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
        {
        }
        public DbSet<VehicleDetails> Vehicles { get; set; }
    }
}
