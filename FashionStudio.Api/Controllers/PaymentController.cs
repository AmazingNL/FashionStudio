using FashionStudio.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FashionStudio.Api.Interfaces;

namespace FashionStudio.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/payment")]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService, IActivityLogService? activityLogService)
            : base(activityLogService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment(
            [FromBody] PaymentRequestDTO request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var payment = await _paymentService.CreatePaymentAsync(request, userId, cancellationToken);
            await LogActivityAsync("Payment", payment.Id, "Created");
            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, payment);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentById(int id, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var payment = await _paymentService.GetPaymentByIdAsync(id, userId, cancellationToken);
            return Ok(payment);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllPayments(
            [FromQuery] QueryParam queryParam,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var payments = await _paymentService.GetAllPaymentsAsync(queryParam, userId, cancellationToken);
            return Ok(payments);
        }
    }
}
