using FashionStudio.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FashionStudio.Api.Interfaces;

namespace FashionStudio.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/order")]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder(
            [FromBody] OrderRequestDTO request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var order = await _orderService.CreateOrderAsync(request, userId, cancellationToken);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            return Ok(order);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllOrders(
            [FromQuery] QueryParam queryParam,
            CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetAllOrdersAsync(queryParam, cancellationToken);
            return Ok(orders);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateOrder(
            int id,
            [FromBody] OrderUpdateDTO request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var order = await _orderService.UpdateOrderAsync(id, request, userId, cancellationToken);
            return Ok(order);
        }

        [HttpPatch("{orderId}/assign/{assignedToUserId}")]
        public async Task<IActionResult> AssignOrderToUser(
            int orderId,
            int assignedToUserId,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var order = await _orderService.AssignOrderToUserAsync(orderId, assignedToUserId, userId, cancellationToken);
            return Ok(order);
        }
    }
}
