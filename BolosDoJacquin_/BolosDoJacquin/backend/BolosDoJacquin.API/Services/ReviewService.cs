using BolosDoJacquin.API.Data;
using BolosDoJacquin.API.DTOs;
using BolosDoJacquin.API.Interfaces;
using BolosDoJacquin.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BolosDoJacquin.API.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _db;

    public ReviewService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<ReviewCreateResult> Create(
        Guid userId,
        Guid productId,
        ReviewInputDto dto)
    {
        var productExists = await _db.Products
            .AnyAsync(p =>
                p.Id == productId &&
                p.Status == RecordStatus.Ativo);

        if (!productExists)
            return ReviewCreateResult.ProductNotFound;

        var reviewExists = await _db.Reviews
            .AnyAsync(r =>
                r.UserId == userId &&
                r.ProductId == productId);

        if (reviewExists)
            return ReviewCreateResult.AlreadyExists;

        var review = new Review
        {
            UserId = userId,
            ProductId = productId,
            Rating = dto.Rating,
            Comment = string.IsNullOrWhiteSpace(dto.Comment)
                ? null
                : dto.Comment.Trim(),
            Status = ReviewStatus.PUBLICADA,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Reviews.Add(review);

        await _db.SaveChangesAsync();

        return ReviewCreateResult.Success;
    }

    public async Task<List<MyReviewDto>> MyReviews(Guid userId)
    {
        return await _db.Reviews
            .AsNoTracking()
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.UpdatedAt)
            .Select(r => new MyReviewDto(
                r.Id,
                r.ProductId,
                r.Product!.Name,
                r.Rating,
                r.Comment,
                r.Status.ToString(),
                r.CreatedAt,
                r.UpdatedAt
            ))
            .ToListAsync();
    }

    public async Task<ReviewUpdateResult> Update(
        Guid userId,
        Guid reviewId,
        ReviewInputDto dto)
    {
        var review = await _db.Reviews
            .FirstOrDefaultAsync(r =>
                r.Id == reviewId &&
                r.UserId == userId);

        if (review is null)
            return ReviewUpdateResult.NotFound;

        review.Rating = dto.Rating;

        review.Comment = string.IsNullOrWhiteSpace(dto.Comment)
            ? null
            : dto.Comment.Trim();

        review.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ReviewUpdateResult.Success;
    }

    public async Task<ReviewDeleteResult> Delete(
        Guid userId,
        Guid reviewId)
    {
        var review = await _db.Reviews
            .FirstOrDefaultAsync(r =>
                r.Id == reviewId &&
                r.UserId == userId);

        if (review is null)
            return ReviewDeleteResult.NotFound;

        _db.Reviews.Remove(review);

        await _db.SaveChangesAsync();

        return ReviewDeleteResult.Success;
    }

    public async Task<List<AdminReviewDto>> Admin(
        Guid? productId,
        Guid? userId,
        int? rating,
        ReviewStatus? status)
    {
        var query = _db.Reviews
            .AsNoTracking()
            .AsQueryable();

        if (productId.HasValue)
        {
            query = query.Where(r =>
                r.ProductId == productId.Value);
        }

        if (userId.HasValue)
        {
            query = query.Where(r =>
                r.UserId == userId.Value);
        }

        if (rating.HasValue)
        {
            query = query.Where(r =>
                r.Rating == rating.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(r =>
                r.Status == status.Value);
        }

        return await query
            .OrderByDescending(r => r.UpdatedAt)
            .Select(r => new AdminReviewDto(
                r.Id,
                r.ProductId,
                r.Product!.Name,
                r.UserId,
                r.User!.Name,
                r.Rating,
                r.Comment,
                r.Status.ToString(),
                r.HiddenReason,
                r.CreatedAt,
                r.UpdatedAt
            ))
            .ToListAsync();
    }

    public async Task<ReviewActionResult> Hide(
        Guid reviewId,
        HideReviewDto dto)
    {
        var review = await _db.Reviews
            .FindAsync(reviewId);

        if (review is null)
            return ReviewActionResult.NotFound;

        review.Status = ReviewStatus.OCULTA;
        review.HiddenReason = dto.Reason.Trim();
        review.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ReviewActionResult.Success;
    }

    public async Task<ReviewActionResult> Restore(Guid reviewId)
    {
        var review = await _db.Reviews
            .FindAsync(reviewId);

        if (review is null)
            return ReviewActionResult.NotFound;

        review.Status = ReviewStatus.PUBLICADA;
        review.HiddenReason = null;
        review.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ReviewActionResult.Success;
    }
}