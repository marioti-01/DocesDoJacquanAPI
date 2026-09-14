using System.ComponentModel.DataAnnotations;
namespace BolosDoJacquin.API.Models;
public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(120)] public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    [MaxLength(500)] public string ImageUrl { get; set; } = string.Empty;
    [MaxLength(240)] public string ShortDescription { get; set; } = string.Empty;
    [MaxLength(2000)] public string LongDescription { get; set; } = string.Empty;
    public bool Available { get; set; } = true;
    public RecordStatus Status { get; set; } = RecordStatus.Ativo;
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
