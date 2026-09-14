using BolosDoJacquin.API.DTOs;
using BolosDoJacquin.API.Interfaces;
using BolosDoJacquin.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BolosDoJacquin.API.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return Guid.Parse(claim!);
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost("product/{productId:guid}")]
    public async Task<ActionResult> Create(
        Guid productId,
        ReviewInputDto dto)
    {
        var userId = GetUserId();

        var result = await _reviewService.Create(
            userId,
            productId,
            dto);

        if (result == ReviewCreateResult.ProductNotFound)
        {
            return NotFound(new
            {
                message = "Produto não encontrado."
            });
        }

        if (result == ReviewCreateResult.AlreadyExists)
        {
            return Conflict(new
            {
                message = "Você já avaliou este produto. Edite sua avaliação existente."
            });
        }

        return Created(
            $"api/reviews/{Guid.NewGuid()}",
            new
            {
                message = "Avaliação criada com sucesso."
            });
    }

    [Authorize(Roles = "Cliente")]
    [HttpGet("mine")]
    public async Task<ActionResult<List<MyReviewDto>>> MyReviews()
    {
        var userId = GetUserId();

        var reviews = await _reviewService.MyReviews(userId);

        return Ok(reviews);
    }

    [Authorize(Roles = "Cliente")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        ReviewInputDto dto)
    {
        var userId = GetUserId();

        var result = await _reviewService.Update(
            userId,
            id,
            dto);

        if (result == ReviewUpdateResult.NotFound)
        {
            return NotFound(new
            {
                message = "Avaliação não encontrada."
            });
        }

        return NoContent();
    }

    [Authorize(Roles = "Cliente")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetUserId();

        var result = await _reviewService.Delete(
            userId,
            id);

        if (result == ReviewDeleteResult.NotFound)
        {
            return NotFound(new
            {
                message = "Avaliação não encontrada."
            });
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet("admin")]
    public async Task<ActionResult<List<AdminReviewDto>>> Admin(
        [FromQuery] Guid? productId,
        [FromQuery] Guid? userId,
        [FromQuery] int? rating,
        [FromQuery] ReviewStatus? status)
    {
        var reviews = await _reviewService.Admin(
            productId,
            userId,
            rating,
            status);

        return Ok(reviews);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id:guid}/hide")]
    public async Task<IActionResult> Hide(
        Guid id,
        HideReviewDto dto)
    {
        var result = await _reviewService.Hide(
            id,
            dto);

        if (result == ReviewActionResult.NotFound)
        {
            return NotFound(new
            {
                message = "Avaliação não encontrada."
            });
        }

        return NoContent();
    }

    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id)
    {
        var result = await _reviewService.Restore(id);

        if (result == ReviewActionResult.NotFound)
        {
            return NotFound(new
            {
                message = "Avaliação não encontrada."
            });
        }

        return NoContent();
    }
}