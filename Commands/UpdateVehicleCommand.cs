using MediatR;
using System.Text.Json.Serialization;
using VehicleMangement.Enums;
using VehicleMangement.Models;

namespace VehicleMangement.Commands
{
    public class UpdateVehicleCommand: IRequest<VehicleDetails>
    {
        [JsonIgnore]
        public Guid VehicleId { get; set; }          
        public string? VehicleNumber { get; set; }  
        public VehicleType? Type { get; set; }       
        public string? LoadMaterial { get; set; }    
        public string? DriverName { get; set; }      
        public string? Source { get; set; }          
        public string? Destination { get; set; }
        public double SourceLat { get; set; }
        public double SourceLng { get; set; }
        public double DestLat { get; set; }
        public double DestLng { get; set; }
    }
}
