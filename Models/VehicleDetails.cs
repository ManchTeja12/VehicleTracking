using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using VehicleMangement.Enums;

namespace VehicleMangement.Models
{
    public class VehicleDetails
    {
        [Key]
        public Guid VehicleId { get; set; }= Guid.NewGuid();
        [Required]
        public string VehicleNumber { get; set; }=string.Empty;
        [Required]
        public VehicleType Type { get; set; }
        public string ShipmentNumber { get; set; }=string.Empty;
        [Required]
        public string LoadMaterial { get; set; }=string.Empty;
        [Required]
        public string DriverName { get; set; }=string.Empty;
        [Required]
        public string Source { get; set; }=string.Empty;
        [Required]
        public string Destination { get; set; } =string.Empty;
        [Required]
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
