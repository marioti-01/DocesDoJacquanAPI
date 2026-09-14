using BolosDoJacquin.API.Data;
using BolosDoJacquin.API.DTOs;
using BolosDoJacquin.API.Interfaces;
using BolosDoJacquin.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BolosDoJacquin.API.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductListDto>> List(
        string? search,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice)
    {
        var query = _db.Products
            .AsNoTracking()
            .Where(p => p.Status == RecordStatus.Ativo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();

            query = query.Where(p =>
                p.Name.ToLower().Contains(term));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p =>
                p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p =>
                p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p =>
                p.Price <= maxPrice.Value);
        }

        var products = await query
            .OrderBy(p => p.Name)
            .Select(p => new ProductListDto(
                p.Id,
                p.Name,
                p.Price,
                p.ImageUrl,
                p.CategoryId,
                p.Category!.Name,
                p.ShortDescription,
                p.Available,
                p.Reviews
                    .Where(r => r.Status == ReviewStatus.PUBLICADA)
                    .Select(r => (double?)r.Rating)
                    .Average(),
                p.Reviews.Count(r =>
                    r.Status == ReviewStatus.PUBLICADA)
            ))
            .ToListAsync();

        return products;
    }

    public async Task<ProductDetailsDto?> Details(Guid id)
    {
        var product = await _db.Products
            .AsNoTracking()
            .Where(p =>
                p.Id == id &&
                p.Status == RecordStatus.Ativo)
            .Select(p => new ProductDetailsDto(
                p.Id,
                p.Name,
                p.Price,
                p.ImageUrl,
                new CategoryDto(
                    p.CategoryId,
                    p.Category!.Name,
                    p.Category.Status.ToString()
                ),
                p.ShortDescription,
                p.LongDescription,
                p.Available,
                p.Reviews
                    .Where(r => r.Status == ReviewStatus.PUBLICADA)
                    .Select(r => (double?)r.Rating)
                    .Average(),
                p.Reviews.Count(r =>
                    r.Status == ReviewStatus.PUBLICADA),
                p.Reviews
                    .Where(r => r.Status == ReviewStatus.PUBLICADA)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => new PublicReviewDto(
                        r.Id,
                        r.Rating,
                        r.Comment,
                        r.User!.Name,
                        r.CreatedAt,
                        r.UpdatedAt
                    ))
            ))
            .FirstOrDefaultAsync();

        return product;
    }

    public async Task<ProductCreateResult> Create(
        ProductInputDto dto)
    {
        var categoryExists = await _db.Categories
            .AnyAsync(c =>
                c.Id == dto.CategoryId &&
                c.Status == RecordStatus.Ativo);

        if (!categoryExists)
        {
            return ProductCreateResult.InvalidCategory;
        }

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Price = dto.Price,
            ImageUrl = dto.ImageUrl.Trim(),
            CategoryId = dto.CategoryId,
            ShortDescription = dto.ShortDescription.Trim(),
            LongDescription = dto.LongDescription.Trim(),
            Available = dto.Available,
            Status = RecordStatus.Ativo
        };

        _db.Products.Add(product);

        await _db.SaveChangesAsync();

        return ProductCreateResult.Success;
    }

    public async Task<ProductUpdateResult> Update(
        Guid id,
        ProductInputDto dto)
    {
        var product = await _db.Products
            .FindAsync(id);

        if (product is null)
        {
            return ProductUpdateResult.NotFound;
        }

        var categoryExists = await _db.Categories
            .AnyAsync(c =>
                c.Id == dto.CategoryId &&
                c.Status == RecordStatus.Ativo);

        if (!categoryExists)
        {
            return ProductUpdateResult.InvalidCategory;
        }

        product.Name = dto.Name.Trim();
        product.Price = dto.Price;
        product.ImageUrl = dto.ImageUrl.Trim();
        product.CategoryId = dto.CategoryId;
        product.ShortDescription = dto.ShortDescription.Trim();
        product.LongDescription = dto.LongDescription.Trim();
        product.Available = dto.Available;

        await _db.SaveChangesAsync();

        return ProductUpdateResult.Success;
    }

    public async Task<ProductActionResult> Availability(
        Guid id,
        bool available)
    {
        var product = await _db.Products
            .FindAsync(id);

        if (product is null)
        {
            return ProductActionResult.NotFound;
        }

        product.Available = available;

        await _db.SaveChangesAsync();

        return ProductActionResult.Success;
    }

    public async Task<ProductActionResult> Deactivate(Guid id)
    {
        var product = await _db.Products
            .FindAsync(id);

        if (product is null)
        {
            return ProductActionResult.NotFound;
        }

        product.Status = RecordStatus.Inativo;

        await _db.SaveChangesAsync();

        return ProductActionResult.Success;
    }

    public async Task<ProductDeleteResult> Delete(Guid id)
    {
        var product = await _db.Products
            .FindAsync(id);

        if (product is null)
        {
            return ProductDeleteResult.NotFound;
        }

        var hasReviews = await _db.Reviews
            .AnyAsync(r => r.ProductId == id);

        if (hasReviews)
        {
            return ProductDeleteResult.HasReviews;
        }

        _db.Products.Remove(product);

        await _db.SaveChangesAsync();

        return ProductDeleteResult.Success;
    }
}