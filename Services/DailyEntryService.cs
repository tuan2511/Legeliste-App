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
        dbEntry.Auslaufzeit1Von = updatedData.Auslaufzeit1Von;
        dbEntry.Auslaufzeit1Bis = updatedData.Auslaufzeit1Bis;
        dbEntry.Auslaufzeit2Von = updatedData.Auslaufzeit2Von;
        dbEntry.Auslaufzeit2Bis = updatedData.Auslaufzeit2Bis;
        dbEntry.Auslaufzeit3Von = updatedData.Auslaufzeit3Von;
        dbEntry.Auslaufzeit3Bis = updatedData.Auslaufzeit3Bis;
        dbEntry.Auslaufzeit4Von = updatedData.Auslaufzeit4Von;
        dbEntry.Auslaufzeit4Bis = updatedData.Auslaufzeit4Bis;
        dbEntry.Kontrollzeit1 = updatedData.Kontrollzeit1;
        dbEntry.Kontrollzeit2 = updatedData.Kontrollzeit2;
        dbEntry.Kontrollzeit3 = updatedData.Kontrollzeit3;
        dbEntry.Kontrollzeit4 = updatedData.Kontrollzeit4;

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
        dbEntry.Auslaufzeit1Von = updatedData.Auslaufzeit1Von;
        dbEntry.Auslaufzeit1Bis = updatedData.Auslaufzeit1Bis;
        dbEntry.Auslaufzeit2Von = updatedData.Auslaufzeit2Von;
        dbEntry.Auslaufzeit2Bis = updatedData.Auslaufzeit2Bis;
        dbEntry.Auslaufzeit3Von = updatedData.Auslaufzeit3Von;
        dbEntry.Auslaufzeit3Bis = updatedData.Auslaufzeit3Bis;
        dbEntry.Auslaufzeit4Von = updatedData.Auslaufzeit4Von;
        dbEntry.Auslaufzeit4Bis = updatedData.Auslaufzeit4Bis;
        dbEntry.Kontrollzeit1 = updatedData.Kontrollzeit1;
        dbEntry.Kontrollzeit2 = updatedData.Kontrollzeit2;
        dbEntry.Kontrollzeit3 = updatedData.Kontrollzeit3;
        dbEntry.Kontrollzeit4 = updatedData.Kontrollzeit4;

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
