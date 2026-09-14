using BolosDoJacquin.API.DTOs;
using BolosDoJacquin.API.Models;

namespace BolosDoJacquin.API.Interfaces;

public interface IReviewService
{
    Task<ReviewCreateResult> Create(
        Guid userId,
        Guid productId,
        ReviewInputDto dto
    );

    Task<List<MyReviewDto>> MyReviews(Guid userId);

    Task<ReviewUpdateResult> Update(
        Guid userId,
        Guid reviewId,
        ReviewInputDto dto
    );

    Task<ReviewDeleteResult> Delete(
        Guid userId,
        Guid reviewId
    );

    Task<List<AdminReviewDto>> Admin(
        Guid? productId,
        Guid? userId,
        int? rating,
        ReviewStatus? status
    );

    Task<ReviewActionResult> Hide(
        Guid reviewId,
        HideReviewDto dto
    );

    Task<ReviewActionResult> Restore(Guid reviewId);
}

public record AdminReviewDto(
    Guid Id,
    Guid ProductId,
    string Product,
    Guid UserId,
    string User,
    int Rating,
    string? Comment,
    string Status,
    string? HiddenReason,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public enum ReviewCreateResult
{
    Success,
    ProductNotFound,
    AlreadyExists
}

public enum ReviewUpdateResult
{
    Success,
    NotFound
}

public enum ReviewDeleteResult
{
    Success,
    NotFound
}

public enum ReviewActionResult
{
    Success,
    NotFound
}