using Microsoft.EntityFrameworkCore;
using VehicleMangement.Models;
namespace VehicleMangement.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options): base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<EventEntity> Events { get; set; }
    }
}
