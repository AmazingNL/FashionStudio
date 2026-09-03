using FashionStudio.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FashionStudio.Api.Interfaces;

namespace FashionStudio.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/customer")]
    public class CustomerController : BaseController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService, IActivityLogService? activityLogService)
            : base(activityLogService)
        {
            _customerService = customerService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCustomer(
            [FromBody] CustomerRequestDTO customerRequest,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var customer = await _customerService.CreateCustomerAsync(customerRequest, userId, cancellationToken);
            await LogActivityAsync("Customer", customer.Id, "Created");
            return Ok(customer);
        }

        [HttpPatch("{customerId}/workspace/{workSpaceId}")]
        public async Task<IActionResult> AssignCustomerToWorkSpace(
            int customerId,
            int workSpaceId,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var customer = await _customerService.AssignCustomerToWorkSpaceAsync(customerId, workSpaceId, userId, cancellationToken);
            await LogActivityAsync("Customer", customerId, "AssignedToWorkSpace");
            return Ok(customer);
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllCustomers(
            [FromQuery] QueryParam queryParam,
            CancellationToken cancellationToken)
        {
            var customers = await _customerService.GetAllCustomersAsync(queryParam, cancellationToken);
            return Ok(customers);
        }

        [HttpPatch("{customerId}/deactivate")]
        public async Task<IActionResult> DeactivateCustomer(int customerId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var customer = await _customerService.DeactivateCustomerAsync(customerId, userId, cancellationToken);
            await LogActivityAsync("Customer", customerId, "Deactivated");
            return Ok(customer);
        }

        [HttpPatch("{customerId}/reactivate")]
        public async Task<IActionResult> ReactivateCustomer(int customerId, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId() ?? throw new InvalidOperationException("User must be logged in");
            var customer = await _customerService.ReactivateCustomerAsync(customerId, userId, cancellationToken);
            await LogActivityAsync("Customer", customerId, "Reactivated");
            return Ok(customer);
        }

    }
}
