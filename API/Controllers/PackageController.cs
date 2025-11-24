using Application.Features.Package.GetAvailablePackages.Queries;
using Application.Features.Package.GetUserPackages.Queries;
using Application.Features.Package.PurchasePackage.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PackageController(IMediator mediator) => _mediator = mediator;

        private int GetUserId() => int.Parse(User.FindFirst("UserId")?.Value ?? "0");

        [HttpGet("available")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailablePackages([FromQuery] string countryCode)
        {
            var query = new GetAvailablePackagesQuery(countryCode);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyPackages()
        {
            var userId = GetUserId();
            var query = new GetUserPackagesQuery(userId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> PurchasePackage([FromBody] PurchasePackageRequest request)
        {
            var userId = GetUserId();

            var command = new PurchasePackageCommand(userId, request.PackageId, request.Price, request.CardNumber);
            var result = await _mediator.Send(command);

            if (result.Contains("failed") || result.Contains("mismatch"))
            {
                return BadRequest(new { Message = result });
            }
            return Ok(new { Message = result });
        }

        public class PurchasePackageRequest
        {
            public int PackageId { get; set; }
            public decimal Price { get; set; }
            public string CardNumber { get; set; }
        }
    }
}
