using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleMangement.Commands;
using VehicleMangement.Dto;
using VehicleMangement.Queries;

namespace VehicleMangement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/command/vehicle")]
    public class VehicleCommandController : ControllerBase
    {
        private readonly IMediator _mediator;
        public VehicleCommandController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateVehicleCommand command)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Unauthorized();
                }
                command.CreatedBy = Guid.Parse(userId).ToString();
                var result = await _mediator.Send(command);
                return Ok(new ApiResponse
                {
                    success = true,
                    message = "Vehicle Created Successfuly",
                    data = result

                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new ApiResponse
                {
                    success = false,
                    message = ex.Message,
                    data = null
                });
            }

        }
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateVehicleCommand command)
        {
            try
            {
                command.VehicleId = id;
                var result = await _mediator.Send(command);
                return Ok(new ApiResponse
                {
                    success = true,
                    message = "Vehicle Updated Successfully",
                    data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse
                {
                    success = false,
                    message = ex.Message,
                    data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    success = false,
                    message = ex.Message,
                    data = null
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var vehicles = await _mediator.Send(new GetAllVehiclesQuery());

            return Ok(new ApiResponse
            {
                success = true,
                message = "Data Retrived Sucessfully",
                data = vehicles
            });
        }

        [HttpGet("{vehicleId}")]
        public async Task<IActionResult> GetById(Guid vehicleId)
        {
            var vehicle = await _mediator.Send(new GetVehicleByIdQuery
            {
                VehicleId = vehicleId
            });

            if (vehicle == null)
                return NotFound("Vehicle not found");

            return Ok(new ApiResponse
            {
                success = true,
                message = "Data Retrived Successfully",
                data = vehicle
            });
        }
    }
}
