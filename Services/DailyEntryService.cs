using LegelisteApp.Data;
using LegelisteApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LegelisteApp.Services;

public class DailyEntryService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;

    public DailyEntryService(IDbContextFactory<AppDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task UpdateEntryAsync(DailyEntry updatedData, ClaimsPrincipal user)
    {
        if (user.IsInRole("Praktikant"))
            throw new UnauthorizedAccessException("Praktikanten dürfen keine Daten ändern.");

        if (updatedData.Date.Date > DateTime.Today)
            throw new InvalidOperationException("Datum darf nicht in der Zukunft liegen!");

        using var context = await _dbFactory.CreateDbContextAsync();
        var dbEntry = await context.DailyEntries.FindAsync(updatedData.Id);
        
        if (dbEntry == null) return;

        // Felder aktualisieren
        dbEntry.Eier1Wahl = updatedData.Eier1Wahl;
        dbEntry.Eier2Wahl = updatedData.Eier2Wahl;
        dbEntry.FutterKg = updatedData.FutterKg;
        dbEntry.WasserLiter = updatedData.WasserLiter;
        dbEntry.FutterlieferungKg = updatedData.FutterlieferungKg;
        dbEntry.Verluste = updatedData.Verluste;
        dbEntry.Eigewicht = updatedData.Eigewicht;
        dbEntry.Koerpergewicht = updatedData.Koerpergewicht;
        dbEntry.Bemerkungen = updatedData.Bemerkungen;
        dbEntry.LichtVon = updatedData.LichtVon;
        dbEntry.LichtBis = updatedData.LichtBis;
        dbEntry.AuslaufzeitMorgensVon = updatedData.AuslaufzeitMorgensVon;
        dbEntry.AuslaufzeitMorgensBis = updatedData.AuslaufzeitMorgensBis;
        dbEntry.AuslaufzeitAbendsVon = updatedData.AuslaufzeitAbendsVon;
        dbEntry.AuslaufzeitAbendsBis = updatedData.AuslaufzeitAbendsBis;
        dbEntry.KontrollzeitenVon = updatedData.KontrollzeitenVon;
        dbEntry.KontrollzeitenBis = updatedData.KontrollzeitenBis;

        // Sicherheitsregel: Wenn Mitarbeiter bearbeitet und es war bereits freigegeben, wird es zurückgesetzt (oder es bleibt Entwurf/Wartend)
        bool isAdmin = user.IsInRole("Admin");
        if (!isAdmin && dbEntry.Status == EntryStatus.Freigegeben)
        {
            dbEntry.Status = EntryStatus.WartetAufFreigabe;
            dbEntry.ApprovedById = null; // Bestehende Freigabe löschen
        }

        await context.SaveChangesAsync();
    }

    public async Task UpdateAndSubmitEntryAsync(DailyEntry updatedData, ClaimsPrincipal user)
    {
        if (user.IsInRole("Praktikant"))
            throw new UnauthorizedAccessException("Praktikanten dürfen keine Daten ändern.");

        if (updatedData.Date.Date > DateTime.Today)
            throw new InvalidOperationException("Datum darf nicht in der Zukunft liegen!");

        using var context = await _dbFactory.CreateDbContextAsync();
        var dbEntry = await context.DailyEntries.FindAsync(updatedData.Id);
        
        if (dbEntry == null) return;

        // Felder aktualisieren
        dbEntry.Eier1Wahl = updatedData.Eier1Wahl;
        dbEntry.Eier2Wahl = updatedData.Eier2Wahl;
        dbEntry.FutterKg = updatedData.FutterKg;
        dbEntry.WasserLiter = updatedData.WasserLiter;
        dbEntry.FutterlieferungKg = updatedData.FutterlieferungKg;
        dbEntry.Verluste = updatedData.Verluste;
        dbEntry.Eigewicht = updatedData.Eigewicht;
        dbEntry.Koerpergewicht = updatedData.Koerpergewicht;
        dbEntry.Bemerkungen = updatedData.Bemerkungen;
        dbEntry.LichtVon = updatedData.LichtVon;
        dbEntry.LichtBis = updatedData.LichtBis;
        dbEntry.AuslaufzeitMorgensVon = updatedData.AuslaufzeitMorgensVon;
        dbEntry.AuslaufzeitMorgensBis = updatedData.AuslaufzeitMorgensBis;
        dbEntry.AuslaufzeitAbendsVon = updatedData.AuslaufzeitAbendsVon;
        dbEntry.AuslaufzeitAbendsBis = updatedData.AuslaufzeitAbendsBis;
        dbEntry.KontrollzeitenVon = updatedData.KontrollzeitenVon;
        dbEntry.KontrollzeitenBis = updatedData.KontrollzeitenBis;

        // Status explizit auf WartetAufFreigabe setzen
        dbEntry.Status = EntryStatus.WartetAufFreigabe;
        dbEntry.ApprovedById = null;

        await context.SaveChangesAsync();
    }

    public async Task<List<DailyEntry>> GetRecentEntriesAsync(int days = 14, int? stallId = null)
    {
        var from = DateTime.Today.AddDays(-days);
        using var context = await _dbFactory.CreateDbContextAsync();
        var query = context.DailyEntries
            .Include(e => e.Stall)
            .Where(e => e.Date >= from);

        if (stallId.HasValue && stallId.Value > 0)
        {
            query = query.Where(e => e.StallId == stallId.Value);
        }

        return await query.OrderByDescending(e => e.Date).ToListAsync();
    }
}
