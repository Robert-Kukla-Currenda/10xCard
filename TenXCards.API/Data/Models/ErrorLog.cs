using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TenXCards.API.Data.Models;

public class ErrorLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int CardId { get; set; }

    [Required]
    public string ErrorDetails { get; set; } = default!;

    [Required]
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey(nameof(CardId))]
    public virtual Card Card { get; set; } = default!;
}