using FashionStudio.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FashionStudio.Api.Interfaces;

namespace FashionStudio.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/measurement")]
    public class MeasurementController : BaseController
    {
        private readonly IMeasurementService _measurementService;

        public MeasurementController(IMeasurementService measurementService, IActivityLogService? activityLogService)
            : base(activityLogService)
        {
            _measurementService = measurementService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateMeasurement(
            [FromBody] MeasurementRequestDTO request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var measurement = await _measurementService.CreateMeasurementAsync(request, userId, cancellationToken);
            await LogActivityAsync("MeasurementSet", measurement.Id, "Created");
            return Ok(measurement);
        }
    }
}
