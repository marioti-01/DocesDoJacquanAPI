using BolosDoJacquin.API.Data;
using BolosDoJacquin.API.DTOs;
using BolosDoJacquin.API.Interfaces;
using BolosDoJacquin.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BolosDoJacquin.API.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryDto>> List()
    {
        return await _db.Categories
            .AsNoTracking()
            .Where(c => c.Status == RecordStatus.Ativo)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Status.ToString()
            ))
            .ToListAsync();
    }

    public async Task<CategoryDto?> Create(CategoryInputDto dto)
    {
        var name = dto.Name.Trim();

        var exists = await _db.Categories
            .AnyAsync(c => c.Name == name);

        if (exists)
            return null;

        var category = new Category
        {
            Name = name,
            Status = RecordStatus.Ativo
        };

        _db.Categories.Add(category);

        await _db.SaveChangesAsync();

        return new CategoryDto(
            category.Id,
            category.Name,
            category.Status.ToString()
        );
    }

    public async Task<CategoryUpdateResult> Update(
        Guid id,
        CategoryInputDto dto)
    {
        var category = await _db.Categories
            .FindAsync(id);

        if (category is null)
            return CategoryUpdateResult.NotFound;

        var name = dto.Name.Trim();

        var exists = await _db.Categories
            .AnyAsync(c =>
                c.Id != id &&
                c.Name == name
            );

        if (exists)
            return CategoryUpdateResult.Duplicate;

        category.Name = name;

        await _db.SaveChangesAsync();

        return CategoryUpdateResult.Success;
    }

    public async Task<CategoryDeleteResult> Delete(Guid id)
    {
        var category = await _db.Categories
            .FindAsync(id);

        if (category is null)
            return CategoryDeleteResult.NotFound;

        var hasProducts = await _db.Products
            .AnyAsync(p => p.CategoryId == id);

        if (hasProducts)
            return CategoryDeleteResult.HasProducts;

        _db.Categories.Remove(category);

        await _db.SaveChangesAsync();

        return CategoryDeleteResult.Success;
    }

    public async Task<bool> Exists(Guid id)
    {
        return await _db.Categories
            .AnyAsync(c => c.Id == id);
    }
}

public enum CategoryUpdateResult
{
    Success,
    NotFound,
    Duplicate
}

public enum CategoryDeleteResult
{
    Success,
    NotFound,
    HasProducts
}