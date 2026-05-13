using System.ComponentModel.DataAnnotations;

namespace LegelisteApp.Data.Models;

public class Stall
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public required string Name { get; set; }

    [Required]
    public int AnfangsbestandTiere { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Einstallungsdatum { get; set; }

    // Navigation property
    public ICollection<DailyEntry> DailyEntries { get; set; } = new List<DailyEntry>();
}
