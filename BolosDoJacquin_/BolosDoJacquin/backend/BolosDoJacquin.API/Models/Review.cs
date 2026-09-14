using System.ComponentModel.DataAnnotations;
namespace BolosDoJacquin.API.Models;
public class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Rating { get; set; }
    [MaxLength(600)] public string? Comment { get; set; }
    public ReviewStatus Status { get; set; } = ReviewStatus.PUBLICADA;
    [MaxLength(400)] public string? HiddenReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
}
