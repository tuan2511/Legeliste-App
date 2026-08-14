using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegelisteApp.Data.Models;

public class FeedDelivery
{
    [Key]
    public int Id { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    /// <summary>
    /// Optional: Dem Stall zugeordnete Lieferung. Null = globale Lieferung für alle Ställe.
    /// </summary>
    public int? StallId { get; set; }

    [ForeignKey(nameof(StallId))]
    public Stall? Stall { get; set; }

    /// <summary>
    /// Gelieferte Menge in Kilogramm.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal AmountKg { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
