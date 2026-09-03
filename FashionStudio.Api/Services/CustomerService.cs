using System;
using FashionStudio.Api.Interfaces;
using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;
using MapsterMapper;
using Mapster;
using FashionStudio.Api.Data;
using FashionStudio.Api.Extensions;
using Microsoft.EntityFrameworkCore;
using FashionStudio.Api.Exceptions;

namespace FashionStudio.Api.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly IWorkSpaceService _workspaceService;
        private readonly IMapper _mapper;

        public CustomerService(IUserService userService, IWorkSpaceService workSpaceService, IMapper mapper, AppDbContext context)
        {
            _userService = userService;
            _workspaceService = workSpaceService;
            _mapper = mapper;
            _context = context;
        }

        public async Task<CustomerResponseDTO> CreateCustomerAsync(CustomerRequestDTO customer, int userId, CancellationToken cancellation)
        {
            var mappedCustomer = _mapper.Map<Customer>(customer);
            Customer? existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Phone == mappedCustomer.Phone, cancellation);
            if (existingCustomer != null)
            {
                throw new ConflictException("A customer with the same Phone Number already exists.");
            }
            var user = await _context.Users.FindAsync(userId, cancellation);
            if (user == null) throw new NotFoundException("user not found");

            mappedCustomer.CreatedByUser = user;
            mappedCustomer.WorkSpace = null;

            await _context.Customers.AddAsync(mappedCustomer, cancellation);
            await _context.SaveChangesAsync(cancellation);
            return _mapper.Map<CustomerResponseDTO>(mappedCustomer);

        }

        public async Task<CustomerResponseDTO> GetCustomerByIdAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                throw new NotFoundException("Customer not found");
            }

            return _mapper.Map<CustomerResponseDTO>(customer);
        }


        public async Task<CustomerResponseDTO> AssignCustomerToWorkSpaceAsync(
            int customerId, int workSpaceId, int requiredUserId, CancellationToken ct)
        {
            await _workspaceService.EnsureIsOwnerOrAssistantAsync(workSpaceId, requiredUserId, ct);

            var customer = await _context.Customers.FindAsync(customerId, ct);
            if (customer == null) throw new NotFoundException("Customer not found");

            customer.WorkSpaceId = workSpaceId;
            await _context.SaveChangesAsync(ct);

            return _mapper.Map<CustomerResponseDTO>(customer);

        }

        public async Task<PageResultDTO<CustomerResponseDTO>> GetAllCustomersAsync(QueryParam queryParam, CancellationToken cancellation)
        {
            var pageDto = await _context.Customers
                .ProjectToType<CustomerResponseDTO>()
                .SearchByAttributes(queryParam.SearchTerm)
                .OrderByProperty(queryParam.SortBy, queryParam.IsDescending)
                .ToPagedListAsync(queryParam, cancellation);
            return pageDto;
        }

        public Task<CustomerResponseDTO> DeactivateCustomerAsync(int customerId, int actingUserId, CancellationToken cancellation) =>
            SetActiveStatusAsync(customerId, isActive: false, actingUserId, cancellation);

        public Task<CustomerResponseDTO> ReactivateCustomerAsync(int customerId, int actingUserId, CancellationToken cancellation) =>
            SetActiveStatusAsync(customerId, isActive: true, actingUserId, cancellation);

        // Helper methods
        private async Task<CustomerResponseDTO> SetActiveStatusAsync(int customerId, bool isActive, int actingUserId, CancellationToken cancellation)
        {
            var customer = await _context.Customers.FindAsync(new object[] { customerId }, cancellation);
            if (customer == null) throw new NotFoundException("Customer not found");

            if (customer.WorkSpaceId != null)
            {
                await _workspaceService.EnsureIsOwnerOrAssistantAsync(customer.WorkSpaceId.Value, actingUserId, cancellation);
            }
            else if (customer.CreatedByUserId != actingUserId)
            {
                // Not yet assigned to any workspace, so there's no Owner/Assistant to defer to —
                // only the person who created this customer record can touch it.
                throw new UnauthorizedAccessException("Only the customer's creator can change its active status before it's assigned to a workspace");
            }

            customer.IsActive = isActive;
            customer.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellation);

            return _mapper.Map<CustomerResponseDTO>(customer);
        }

    }
}
