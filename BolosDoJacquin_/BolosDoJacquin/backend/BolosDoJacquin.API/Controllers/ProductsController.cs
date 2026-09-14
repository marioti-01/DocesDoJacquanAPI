using BolosDoJacquin.API.DTOs;
using BolosDoJacquin.API.Interfaces;
using BolosDoJacquin.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BolosDoJacquin.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<ProductListDto>>> List(
        [FromQuery] string? search,
        [FromQuery] Guid? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice)
    {
        var products = await _productService.List(
            search,
            categoryId,
            minPrice,
            maxPrice
        );

        return Ok(products);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDetailsDto>> Details(
        Guid id)
    {
        var product = await _productService.Details(id);

        if (product is null)
        {
            return NotFound(new
            {
                message = "Produto não encontrado."
            });
        }

        return Ok(product);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<ActionResult> Create(
        ProductInputDto dto)
    {
        var result = await _productService.Create(dto);

        if (result == ProductCreateResult.InvalidCategory)
        {
            return BadRequest(new
            {
                message = "Categoria inválida."
            });
        }

        return Ok(new
        {
            message = "Produto criado com sucesso."
        });
    }

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        ProductInputDto dto)
    {
        var result = await _productService.Update(id, dto);

        if (result == ProductUpdateResult.NotFound)
        {
            return NotFound(new
            {
                message = "Produto não encontrado."
            });
        }

        if (result == ProductUpdateResult.InvalidCategory)
        {
            return BadRequest(new
            {
                message = "Categoria inválida."
            });
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id:guid}/availability")]
    public async Task<IActionResult> Availability(
        Guid id,
        [FromBody] bool available)
    {
        var result = await _productService.Availability(
            id,
            available
        );

        if (result == ProductActionResult.NotFound)
        {
            return NotFound(new
            {
                message = "Produto não encontrado."
            });
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(
        Guid id)
    {
        var result = await _productService.Deactivate(id);

        if (result == ProductActionResult.NotFound)
        {
            return NotFound(new
            {
                message = "Produto não encontrado."
            });
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _productService.Delete(id);

        if (result == ProductDeleteResult.NotFound)
        {
            return NotFound(new
            {
                message = "Produto não encontrado."
            });
        }

        if (result == ProductDeleteResult.HasReviews)
        {
            return Conflict(new
            {
                message = "Produto possui avaliações; desative-o em vez de excluir."
            });
        }

        return NoContent();
    }
}