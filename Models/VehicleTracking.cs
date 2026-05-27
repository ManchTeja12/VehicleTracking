using System.ComponentModel.DataAnnotations;

namespace VehicleMangement.Models
{
    public class VehicleTracking
    {
        [Key]
        public int Id { get; set; }
        public Guid VehicleId { get; set; }
        public double SourceLat { get; set; }
        public double SourceLng { get; set; }
        public double DestLat { get; set; }
        public double DestLng { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
