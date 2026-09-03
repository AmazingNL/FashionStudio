using FashionStudio.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FashionStudio.Api.Interfaces;

namespace FashionStudio.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/fitting")]
    public class FittingController : BaseController
    {
        private readonly IFittingService _fittingService;

        public FittingController(IFittingService fittingService, IActivityLogService? activityLogService)
            : base(activityLogService)
        {
            _fittingService = fittingService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateFitting(
            [FromBody] FittingRequestDTO request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var fitting = await _fittingService.CreateFittingAsync(request, userId, cancellationToken);
            await LogActivityAsync("Fitting", fitting.Id, "Created");
            return CreatedAtAction(nameof(GetFittingById), new { id = fitting.Id }, fitting);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFittingById(int id)
        {
            var fitting = await _fittingService.GetFittingByIdAsync(id);
            return Ok(fitting);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllFittings(
            [FromQuery] QueryParam queryParam,
            CancellationToken cancellationToken)
        {
            var fittings = await _fittingService.GetAllFittingsAsync(queryParam, cancellationToken);
            return Ok(fittings);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateFitting(
            int id,
            [FromBody] FittingUpdateDTO request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var fitting = await _fittingService.UpdateFittingAsync(id, request, userId, cancellationToken);
            await LogActivityAsync("Fitting", id, "Updated");
            return Ok(fitting);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFitting(int id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            await _fittingService.DeleteFittingAsync(id, userId, cancellationToken);
            await LogActivityAsync("Fitting", id, "Deleted");
            return NoContent();
        }
    }
}
