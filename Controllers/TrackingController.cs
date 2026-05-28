using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using VehicleMangement.Hubs;
using VehicleMangement.Queries;

namespace VehicleMangement.Controllers
{
    [ApiController]
    [Route("api/tracking")]
    public class TrackingController: ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<RouteHub> _hubContext;

        public TrackingController(IMediator mediator, IHubContext<RouteHub> hubContext)
        {
            _mediator = mediator;
            _hubContext = hubContext;
        }
        [HttpPost("start/{vehicleId}")]
        public async Task<IActionResult> StartTracking(string vehicleId)
        {
            if (!Guid.TryParse(vehicleId, out var id))
            {
                return BadRequest("Invalid Vehicle Id");
            }
            var vehicle = await _mediator.Send(new GetVehicleByIdQuery { VehicleId = id });

            if (vehicle == null)
                return NotFound("Vehicle Not Found");

            await _hubContext.Clients .Group(vehicleId).SendAsync("StartTrip",vehicleId);

            return Ok("Tracking Started");
        }
    }

}
