using MediatR;
using Microsoft.AspNetCore.Mvc;
using VehicleMangement.Commands;
using VehicleMangement.Dto;
using VehicleMangement.Queries;

namespace VehicleMangement.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("SignUp")]
        public async Task<IActionResult> Signup(SignUpCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(new ApiResponse
                {
                    success = true,
                    message = "User created sucessfully",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse
                {
                    success = false,
                    message = ex.Message,
                    data = null
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

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginQuery query)
        {
            try
            {
                var result = await _mediator.Send(query);
                return Ok(new ApiResponse
                {
                    success = true,
                    message = "Login successful",
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
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ApiResponse
                {
                    success = false,
                    message = ex.Message,
                    data = null
                });
            }
        }
    }
}
