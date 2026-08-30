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
            var membership = await 
                _context.WorkSpaceMemberships.FirstOrDefaultAsync(
                    m => m.WorkSpaceId == workSpaceId 
                    && m.UserId == requiredUserId, ct
                    );
            if (membership == null 
                || (membership.Role != Role.Owner 
                && membership.Role != Role.Assistant))
            {
                throw new UnauthorizedAccessException("User not allow to add customer, you must be an Owner or Assistant");
            }

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

    }
}
