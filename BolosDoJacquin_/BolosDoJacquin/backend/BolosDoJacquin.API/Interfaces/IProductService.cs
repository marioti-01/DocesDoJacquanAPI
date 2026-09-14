using BolosDoJacquin.API.DTOs;

namespace BolosDoJacquin.API.Interfaces;

public interface IProductService
{
    Task<List<ProductListDto>> List(
        string? search,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice
    );

    Task<ProductDetailsDto?> Details(Guid id);

    Task<ProductCreateResult> Create(ProductInputDto dto);

    Task<ProductUpdateResult> Update(
        Guid id,
        ProductInputDto dto
    );

    Task<ProductActionResult> Availability(
        Guid id,
        bool available
    );

    Task<ProductActionResult> Deactivate(Guid id);

    Task<ProductDeleteResult> Delete(Guid id);
}

public enum ProductCreateResult
{
    Success,
    InvalidCategory
}

public enum ProductUpdateResult
{
    Success,
    NotFound,
    InvalidCategory
}

public enum ProductActionResult
{
    Success,
    NotFound
}

public enum ProductDeleteResult
{
    Success,
    NotFound,
    HasReviews
}