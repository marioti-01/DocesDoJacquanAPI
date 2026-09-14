using BolosDoJacquin.API.DTOs;
using BolosDoJacquin.API.Services;

namespace BolosDoJacquin.API.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryDto>> List();

    Task<CategoryDto?> Create(CategoryInputDto dto);

    Task<CategoryUpdateResult> Update(
        Guid id,
        CategoryInputDto dto
    );

    Task<CategoryDeleteResult> Delete(Guid id);

    Task<bool> Exists(Guid id);
}