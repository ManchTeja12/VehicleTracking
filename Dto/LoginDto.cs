using System.ComponentModel.DataAnnotations;

namespace VehicleMangement.Dto
{
    public class LoginDto
    {
        [Required]
        public string? Email { get; set; }=string.Empty;
        [Required]
        public string? Password { get; set; } = string.Empty;
    }
}
