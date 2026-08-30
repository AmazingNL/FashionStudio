using FashionStudio.Api.Data;
using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;
using FashionStudio.Api.Interfaces;
using FashionStudio.Api.Exceptions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace FashionStudio.Api.Services
{
    public class MeasurementService : IMeasurementService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MeasurementService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MeasurementSetDTO> CreateMeasurementAsync(MeasurementRequestDTO request, int actingUserId, CancellationToken cancellation)
        {
            var customer = await _context.Customers.FindAsync(new object[] { request.CustomerId }, cancellation);
            if (customer == null) throw new NotFoundException("Customer not found");

            var user = await _context.Users.FindAsync(new object[] { actingUserId }, cancellation);
            if (user == null) throw new NotFoundException("User not found");

            var measurementSet = new MeasurementSet
            {
                CustomerId = customer.Id,
                WorkSpaceId = customer.WorkSpaceId,
                CreatedByUserId = user.Id,
                label = request.Label,
                Note = request.Note,
                Unit = request.Unit,
                DateTaken = request.DateTaken,
                UpdatedAt = DateTime.UtcNow,
            };

            var field = _mapper.Map<MeasurementFiled>(request.Fields);
            measurementSet.MeasurementFiled.Add(field);

            await _context.MeasurementSets.AddAsync(measurementSet, cancellation);
            await _context.SaveChangesAsync(cancellation);

            return _mapper.Map<MeasurementSetDTO>(measurementSet);
        }
    }
}
