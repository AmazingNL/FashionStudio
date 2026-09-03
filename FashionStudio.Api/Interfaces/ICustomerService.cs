using FashionStudio.Api.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FashionStudio.Api.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponseDTO> CreateCustomerAsync(CustomerRequestDTO customer, int userId, CancellationToken cancellation);
        Task<CustomerResponseDTO> GetCustomerByIdAsync(int customerId, int actingUserId, CancellationToken cancellation);
        Task<CustomerResponseDTO> AssignCustomerToWorkSpaceAsync(int customerId, int workSpaceId, int actingUserId, CancellationToken cancellation);
        Task<PageResultDTO<CustomerResponseDTO>> GetAllCustomersAsync(QueryParam queryParam, int actingUserId, CancellationToken cancellation);
        Task<CustomerResponseDTO> DeactivateCustomerAsync(int customerId, int actingUserId, CancellationToken cancellation);
        Task<CustomerResponseDTO> ReactivateCustomerAsync(int customerId, int actingUserId, CancellationToken cancellation);

    }
}
