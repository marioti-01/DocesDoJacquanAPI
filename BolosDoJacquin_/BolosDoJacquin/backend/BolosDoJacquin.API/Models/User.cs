using System.ComponentModel.DataAnnotations;

namespace BolosDoJacquin.API.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [MaxLength(120)] public string Name { get; set; } = string.Empty;
    [MaxLength(180)] public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Cliente;
    public RecordStatus Status { get; set; } = RecordStatus.Ativo;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
