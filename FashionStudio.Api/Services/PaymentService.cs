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
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWorkSpaceService _workSpaceService;

        public PaymentService(AppDbContext context, IMapper mapper, IWorkSpaceService workSpaceService)
        {
            _context = context;
            _mapper = mapper;
            _workSpaceService = workSpaceService;
        }

        public async Task<PaymentResponseDTO> CreatePaymentAsync(PaymentRequestDTO request, int userId, CancellationToken cancellation)
        {
            if (request.Amount <= 0) throw new ConflictException("Payment amount must be greater than zero");

            var order = await _context.Orders.FindAsync(new object[] { request.OrderId }, cancellation);
            if (order == null) throw new NotFoundException("Order not found");

            await _workSpaceService.EnsureIsOwnerOrAssistantAsync(order.WorkSpaceId, userId, cancellation);

            var alreadyPaid = await _context.Payments
                .Where(p => p.OrderId == order.Id)
                .SumAsync(p => p.Amount, cancellation);

            var balanceDue = order.QuotedPrice - order.Discount - alreadyPaid;
            if (request.Amount > balanceDue)
                throw new ConflictException($"Payment of {request.Amount} exceeds the remaining balance of {balanceDue}");

            var payment = _mapper.Map<Payment>(request);
            payment.WorkSpaceId = order.WorkSpaceId;
            payment.CreatedByUserId = userId;

            await _context.Payments.AddAsync(payment, cancellation);
            await _context.SaveChangesAsync(cancellation);

            return _mapper.Map<PaymentResponseDTO>(payment);
        }

        public async Task<PaymentResponseDTO> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) throw new NotFoundException("Payment not found");

            return _mapper.Map<PaymentResponseDTO>(payment);
        }

        public async Task<PageResultDTO<PaymentResponseDTO>> GetAllPaymentsAsync(QueryParam queryParam, CancellationToken cancellation)
        {
            var pageDto = await _context.Payments
                .ProjectToType<PaymentResponseDTO>()
                .SearchByAttributes(queryParam.SearchTerm)
                .OrderByProperty(queryParam.SortBy, queryParam.IsDescending)
                .ToPagedListAsync(queryParam, cancellation);
            return pageDto;
        }
    }
}
