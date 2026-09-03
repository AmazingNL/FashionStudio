using FashionStudio.Api.Interfaces;
using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;
using FashionStudio.Api.Configuration;
using MapsterMapper;
using Mapster;
using FashionStudio.Api.Data;
using FashionStudio.Api.Extensions;
using FashionStudio.Api.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FashionStudio.Api.Services
{
    public class OrderImageService : IOrderImageService
    {
        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/webp"
        };

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWorkSpaceService _workSpaceService;
        private readonly IWebHostEnvironment _environment;
        private readonly StorageSettings _storageSettings;

        public OrderImageService(
            AppDbContext context,
            IMapper mapper,
            IWorkSpaceService workSpaceService,
            IWebHostEnvironment environment,
            IOptions<StorageSettings> storageSettings)
        {
            _context = context;
            _mapper = mapper;
            _workSpaceService = workSpaceService;
            _environment = environment;
            _storageSettings = storageSettings.Value;
        }

        public async Task<OrderImageResponseDTO> UploadImageAsync(OrderImageUploadDTO request, int userId, CancellationToken cancellation)
        {
            if (request.File == null || request.File.Length == 0)
                throw new ValidationException("No file was uploaded");
            if (request.File.Length > _storageSettings.MaxFileSizeBytes)
                throw new ValidationException($"File exceeds the {_storageSettings.MaxFileSizeBytes / (1024 * 1024)}MB limit");
            if (!AllowedContentTypes.Contains(request.File.ContentType))
                throw new ValidationException("Only JPEG, PNG, or WEBP images are allowed");

            var order = await _context.Orders.FindAsync(new object[] { request.OrderId }, cancellation);
            if (order == null) throw new NotFoundException("Order not found");

            await _workSpaceService.EnsureIsMemberAsync(order.WorkSpaceId, userId, cancellation);

            var folder = Path.Combine(_environment.ContentRootPath, _storageSettings.OrderImagesPath);
            Directory.CreateDirectory(folder);

            var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.File.FileName)}";
            var fullPath = Path.Combine(folder, storedFileName);

            await using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream, cancellation);
            }

            var image = new OrderImage
            {
                OrderId = order.Id,
                WorkSpaceId = order.WorkSpaceId,
                UserId = userId,
                Title = request.Title,
                Description = request.Description,
                StoredFileName = storedFileName,
                ContentType = request.File.ContentType,
                CreatedAt = DateTime.UtcNow
            };

            await _context.OrderImages.AddAsync(image, cancellation);
            await _context.SaveChangesAsync(cancellation);

            return ToResponseDto(image);
        }

        public async Task<OrderImageResponseDTO> GetImageByIdAsync(int imageId)
        {
            var image = await _context.OrderImages.FindAsync(imageId);
            if (image == null) throw new NotFoundException("Image not found");

            return ToResponseDto(image);
        }

        public async Task<(Stream Stream, string ContentType, string FileName)> GetImageFileAsync(int imageId, CancellationToken cancellation)
        {
            var image = await _context.OrderImages.FindAsync(new object[] { imageId }, cancellation);
            if (image == null) throw new NotFoundException("Image not found");

            var folder = Path.Combine(_environment.ContentRootPath, _storageSettings.OrderImagesPath);
            var fullPath = Path.Combine(folder, Path.GetFileName(image.StoredFileName));
            if (!File.Exists(fullPath)) throw new NotFoundException("Image file is missing from storage");

            Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return (stream, image.ContentType, image.StoredFileName);
        }

        public async Task<PageResultDTO<OrderImageResponseDTO>> GetAllImagesAsync(QueryParam queryParam, CancellationToken cancellation)
        {
            var pageDto = await _context.OrderImages
                .ProjectToType<OrderImageResponseDTO>()
                .SearchByAttributes(queryParam.SearchTerm)
                .OrderByProperty(queryParam.SortBy, queryParam.IsDescending)
                .ToPagedListAsync(queryParam, cancellation);

            if (pageDto.Items != null)
            {
                foreach (var item in pageDto.Items)
                {
                    item.DownloadUrl = $"/api/orderimage/{item.Id}/file";
                }
            }

            return pageDto;
        }

        public async Task DeleteImageAsync(int imageId, int actingUserId, CancellationToken cancellation)
        {
            var image = await _context.OrderImages.FindAsync(new object[] { imageId }, cancellation);
            if (image == null) throw new NotFoundException("Image not found");

            // The uploader can remove their own photo; otherwise it takes Owner/Assistant.
            if (image.UserId != actingUserId)
            {
                await _workSpaceService.EnsureIsOwnerOrAssistantAsync(image.WorkSpaceId, actingUserId, cancellation);
            }

            DeleteFileIfExists(image.StoredFileName);

            _context.OrderImages.Remove(image);
            await _context.SaveChangesAsync(cancellation);
        }

        public async Task DeleteImagesForOrderAsync(int orderId, CancellationToken cancellation)
        {
            // Only cleans up the files on disk — the OrderImage rows themselves are removed by
            // the DB's ON DELETE CASCADE from Orders when the caller (OrderService) deletes the
            // order itself, so there's no matching _context.OrderImages.Remove(...) here.
            var images = await _context.OrderImages
                .Where(i => i.OrderId == orderId)
                .ToListAsync(cancellation);

            foreach (var image in images)
            {
                DeleteFileIfExists(image.StoredFileName);
            }
        }

        // Helper methods
        private OrderImageResponseDTO ToResponseDto(OrderImage image)
        {
            var dto = _mapper.Map<OrderImageResponseDTO>(image);
            dto.DownloadUrl = $"/api/orderimage/{image.Id}/file";
            return dto;
        }

        private void DeleteFileIfExists(string storedFileName)
        {
            var folder = Path.Combine(_environment.ContentRootPath, _storageSettings.OrderImagesPath);
            var fullPath = Path.Combine(folder, Path.GetFileName(storedFileName));
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
    }
}
