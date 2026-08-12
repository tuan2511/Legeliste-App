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

    [Required]
    public int StallId { get; set; }

    [ForeignKey(nameof(StallId))]
    public Stall? Stall { get; set; }

    [Required]
    public decimal AmountTons { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
