using BolosDoJacquin.API.DTOs;
using BolosDoJacquin.API.Interfaces;
using BolosDoJacquin.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolosDoJacquin.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> List()
    {
        var categories = await _categoryService.List();

        return Ok(categories);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create(
        CategoryInputDto dto)
    {
        var category = await _categoryService.Create(dto);

        if (category is null)
        {
            return Conflict(new
            {
                message = "Categoria já existe."
            });
        }

        return CreatedAtAction(
            nameof(List),
            new { id = category.Id },
            category
        );
    }

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        CategoryInputDto dto)
    {
        var result = await _categoryService.Update(id, dto);

        if (result == CategoryUpdateResult.NotFound)
        {
            return NotFound(new
            {
                message = "Categoria não encontrada."
            });
        }

        if (result == CategoryUpdateResult.Duplicate)
        {
            return Conflict(new
            {
                message = "Categoria já existe."
            });
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _categoryService.Delete(id);

        if (result == CategoryDeleteResult.NotFound)
        {
            return NotFound(new
            {
                message = "Categoria não encontrada."
            });
        }

        if (result == CategoryDeleteResult.HasProducts)
        {
            return Conflict(new
            {
                message = "Não é possível excluir categoria com produtos associados."
            });
        }

        return NoContent();
    }
}