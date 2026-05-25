using MediatR;
using VehicleMangement.Models;

namespace VehicleMangement.Queries
{
    public class LoginQuery : IRequest<LoginResponse>
    {
        public string Email { get; set; }=string.Empty;
        public string Password { get; set; }=string.Empty;

    }
}
