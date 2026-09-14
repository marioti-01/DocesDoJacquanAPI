using System.ComponentModel.DataAnnotations;
namespace BolosDoJacquin.API.Models;
public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(80)] public string Name { get; set; } = string.Empty;
    public RecordStatus Status { get; set; } = RecordStatus.Ativo;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
