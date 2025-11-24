using Application.Features.Auth.Login.Commands;
using Application.Features.Auth.Password.Commands;
using Application.Features.Auth.Profile.Queries;
using Application.Features.Auth.Register.Command;
using Application.Features.Auth.VerifyEmail.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId) && userId > 0)
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand query)
        {
            var token = await _mediator.Send(query);

            if (token == "Email is not Verified")
            {
                return BadRequest(new { Message = token });
            }

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { Message = "Invalid email or password." });
            }

            return Ok(new { Token = token, Message = "Login successful." });
        }

        [HttpGet("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Missing verification token.");
            }

            var command = new VerifyEmailCommand { Token = token };
            var success = await _mediator.Send(command);

            if (success)
            {
                return Ok("Email successfully verified! You can now log in.");
            }

            return BadRequest("Invalid or expired verification link.");
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetUserId();
                var query = new GetProfileQuery(userId);
                var profile = await _mediator.Send(query);

                if (profile == null)
                {
                    return NotFound(new { Message = "User profile not found." });
                }

                return Ok(profile);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching the profile.", Details = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            try
            {
                command.UserId = GetUserId();

                await _mediator.Send(command);

                return Ok(new { Message = "Password changed successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while changing the password.", Details = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);

            return Ok(new { message = "Password reset initiated.", Token = result });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            try
            {
                await _mediator.Send(command);

                return Ok(new { Message = "Password successfully reset. You can now log in with your new password." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred during password reset.", Details = ex.Message });
            }
        }
    }
}
