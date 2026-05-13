using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegelisteApp.Data.Models;

public class DailyEntry
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
    public int CreatorId { get; set; }

    [ForeignKey(nameof(CreatorId))]
    public User? Creator { get; set; }

    [Required]
    public EntryStatus Status { get; set; } = EntryStatus.Entwurf;

    public int? ApprovedById { get; set; }

    [ForeignKey(nameof(ApprovedById))]
    public User? ApprovedBy { get; set; }

    [Required]
    public int Verluste { get; set; }
    
    [Required]
    public int Eier1Wahl { get; set; }
    
    [Required]
    public int Eier2Wahl { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal FutterKg { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal WasserLiter { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? FutterlieferungKg { get; set; }
    
    [MaxLength(500)]
    public string? Bemerkungen { get; set; }
    
    [MaxLength(50)]
    public string? Auslaufzeit { get; set; }
    
    public string? LichtVon { get; set; }
    
    public string? LichtBis { get; set; }
    
    public decimal? Eigewicht { get; set; }
    
    public decimal? Koerpergewicht { get; set; }
    
    public int? ZugaengeTiere { get; set; }
    
    [MaxLength(20)]
    public string? KontrollzeitenVon { get; set; }
    
    [MaxLength(20)]
    public string? KontrollzeitenBis { get; set; }
}
