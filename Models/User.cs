using System.ComponentModel.DataAnnotations;

namespace VehicleMangement.Models
{
    public class User
    {
       
        [Key]
        public Guid UserId { get; set; }= Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
