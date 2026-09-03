using FashionStudio.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FashionStudio.Api.Interfaces;

namespace FashionStudio.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/orderimage")]
    public class OrderImageController : BaseController
    {
        private readonly IOrderImageService _orderImageService;

        public OrderImageController(IOrderImageService orderImageService, IActivityLogService? activityLogService)
            : base(activityLogService)
        {
            _orderImageService = orderImageService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(
            [FromForm] OrderImageUploadDTO request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var image = await _orderImageService.UploadImageAsync(request, userId, cancellationToken);
            await LogActivityAsync("OrderImage", image.Id, "Uploaded");
            return CreatedAtAction(nameof(GetImageById), new { id = image.Id }, image);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImageById(int id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var image = await _orderImageService.GetImageByIdAsync(id, userId, cancellationToken);
            return Ok(image);
        }

        [HttpGet("{id}/file")]
        public async Task<IActionResult> GetImageFile(int id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var (stream, contentType, fileName) = await _orderImageService.GetImageFileAsync(id, userId, cancellationToken);
            return File(stream, contentType, fileName);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllImages(
            [FromQuery] QueryParam queryParam,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var images = await _orderImageService.GetAllImagesAsync(queryParam, userId, cancellationToken);
            return Ok(images);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            await _orderImageService.DeleteImageAsync(id, userId, cancellationToken);
            await LogActivityAsync("OrderImage", id, "Deleted");
            return NoContent();
        }
    }
}
