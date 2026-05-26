using MediatR;
using System.ComponentModel.DataAnnotations;
using VehicleMangement.Models;

namespace VehicleMangement.Queries
{
    public class LoginQuery : IRequest<LoginResponse>
    {
        [Required]
        public string Email { get; set; }=string.Empty;
        [Required]
        public string Password { get; set; }=string.Empty;

    }
}
