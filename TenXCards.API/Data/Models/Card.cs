using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenXCards.API.Data.Models;

public class Card
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MinLength(1000)]
    [MaxLength(10000)]
    public string OriginalContent { get; set; } = default!;

    [Required]
    [MinLength(1)]
    [MaxLength(1000)]
    public string Front { get; set; } = default!;

    [Required]
    [MinLength(1)]
    [MaxLength(5000)]
    public string Back { get; set; } = default!;

    [Required]
    [StringLength(10)]
    public string GeneratedBy { get; set; } = default!;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = default!;
    
    public virtual ICollection<ErrorLog> ErrorLogs { get; set; } = new List<ErrorLog>();
}