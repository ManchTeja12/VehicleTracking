using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VehicleMangement.Data;
using VehicleMangement.Hubs;

namespace VehicleMangement.Controllers
{
    [ApiController]
    [Route("api/tracking")]
    public class TrackingController: ControllerBase
    {
        private readonly ReadDbContext _context;
        private readonly IHubContext<RouteHub> _hubContext;
        private readonly HttpClient _httpClient;

        public TrackingController(ReadDbContext context, IHubContext<RouteHub> hubContext, HttpClient httpClient)
        {
            _context = context;
            _hubContext = hubContext;
           // _httpClient = httpClient;
        }
        [HttpPost("start/{vehicleId}")]
        public async Task<IActionResult> StartTracking(string vehicleId)
        {
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v =>v.VehicleId == Guid.Parse(vehicleId));

            if (vehicle == null)
                return NotFound("Vehicle Not Found");

            await _hubContext.Clients .Group(vehicleId).SendAsync("StartTrip",vehicleId);

            return Ok("Tracking Started");
        }
    }

}
