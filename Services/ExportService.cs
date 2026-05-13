using System.Globalization;
using ClosedXML.Excel;
using LegelisteApp.Data;
using LegelisteApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegelisteApp.Services;

public class ExportService
{
    private readonly IDbContextFactory<AppDbContext> _dbFactory;
    private readonly FlockCalculationService _calcService;

    public ExportService(IDbContextFactory<AppDbContext> dbFactory, FlockCalculationService calcService)
    {
        _dbFactory = dbFactory;
        _calcService = calcService;
    }

    public async Task<byte[]> GenerateExcelAsync(List<DailyEntry> entries)
    {
        using var workbook = new XLWorkbook();
        using var context = await _dbFactory.CreateDbContextAsync();

        // Gruppiere Einträge nach Stall, da jeder Stall ein eigenes Blatt bekommt
        var stalls = entries.Select(e => e.Stall).DistinctBy(s => s?.Id).ToList();

        foreach (var stall in stalls)
        {
            if (stall == null) continue;

            var stallEntries = entries.Where(e => e.StallId == stall.Id).OrderBy(e => e.Date).ToList();
            if (!stallEntries.Any()) continue;

            var ws = workbook.Worksheets.Add(stall.Name.Length > 31 ? stall.Name.Substring(0, 31) : stall.Name);

            // Setup Header Row
            string[] headers = { "Datum", "Hühner", "Verluste", "Zugänge", "Eier 1e Wahl", "Eier 2e Wahl", "Eier Gesamt", "Leistung %", "Futter kg", "Futter Lieferung", "Futter g/Tier", "Wasser Liter", "Wasser ml/Tier", "Bemerkungen", "Auslauf Zeit" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Fill.BackgroundColor = XLColor.Yellow;
                cell.Style.Font.Bold = true;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int row = 2;
            int blockStartRow = 2;
            
            // Woche/Block Tracking
            int tageImBlock = 0;
            decimal weeklyLosses = 0;
            decimal weeklyZugaenge = 0;
            int weeklyEggs1 = 0;
            int weeklyEggs2 = 0;
            int weeklyEggsTotal = 0;
            decimal weeklyFutter = 0;
            decimal weeklyWasser = 0;

            for (int i = 0; i < stallEntries.Count; i++)
            {
                var entry = stallEntries[i];
                
                // Bestand & Verluste berechnen (unter Berücksichtigung von Zugängen)
                var pastChanges = await context.DailyEntries
                    .Where(e => e.StallId == entry.StallId && e.Date < entry.Date && e.Status == EntryStatus.Freigegeben)
                    .SumAsync(e => e.Verluste - (e.ZugaengeTiere ?? 0));
                
                int totalNetLossesSoFar = pastChanges + (entry.Verluste - (entry.ZugaengeTiere ?? 0));
                int currentFlockSize = stall.AnfangsbestandTiere - pastChanges;
                decimal verlustProzent = stall.AnfangsbestandTiere > 0 ? ((decimal)totalNetLossesSoFar / stall.AnfangsbestandTiere) * 100 : 0;
                
                int todayLosses = entry.Verluste;
                int todayZugaenge = entry.ZugaengeTiere ?? 0;
                int totalEggs = entry.Eier1Wahl + entry.Eier2Wahl;
                
                var legeleistung = _calcService.CalculateLayingPerformance(totalEggs, currentFlockSize);
                var futterProTier = _calcService.CalculateFeedPerBird(entry.FutterKg, currentFlockSize);
                var wasserProTier = _calcService.CalculateWaterPerBird(entry.WasserLiter, currentFlockSize);
                var wasserFutterRatio = _calcService.CalculateWaterFeedRatio(entry.WasserLiter, entry.FutterKg);

                // Werte schreiben
                ws.Cell(row, 1).Value = entry.Date.ToShortDateString();
                ws.Cell(row, 2).Value = currentFlockSize;
                ws.Cell(row, 3).Value = todayLosses;
                ws.Cell(row, 4).Value = todayZugaenge;
                ws.Cell(row, 5).Value = entry.Eier1Wahl;
                ws.Cell(row, 6).Value = entry.Eier2Wahl;
                ws.Cell(row, 7).Value = totalEggs;
                ws.Cell(row, 8).Value = legeleistung;
                ws.Cell(row, 9).Value = entry.FutterKg;
                ws.Cell(row, 10).Value = entry.FutterlieferungKg;
                ws.Cell(row, 11).Value = futterProTier;
                ws.Cell(row, 12).Value = entry.WasserLiter;
                ws.Cell(row, 13).Value = wasserProTier;
                ws.Cell(row, 14).Value = entry.Bemerkungen;
                ws.Cell(row, 15).Value = entry.Auslaufzeit;

                // Rahmen für die Zeile
                ws.Range(row, 1, row, 15).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Range(row, 1, row, 15).Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // Akkumulieren für die Woche
                weeklyLosses += todayLosses;
                weeklyZugaenge += todayZugaenge;
                weeklyEggs1 += entry.Eier1Wahl;
                weeklyEggs2 += entry.Eier2Wahl;
                weeklyEggsTotal += totalEggs;
                weeklyFutter += entry.FutterKg;
                weeklyWasser += entry.WasserLiter;
                
                // Seiten-Block Logik (rechts von Auslauf Zeit, also ab Spalte P=16)
                if (tageImBlock == 0) // Erste Zeile im 7-Tage Block
                {
                    ws.Cell(row, 16).Value = "Licht";
                    ws.Cell(row, 17).Value = $"{entry.LichtVon}-{entry.LichtBis}";
                    ws.Cell(row, 17).Style.Fill.BackgroundColor = XLColor.Yellow;
                    ws.Cell(row, 18).Value = "Uhr";
                }
                else if (tageImBlock == 1)
                {
                    ws.Cell(row, 16).Value = "Eigewicht";
                    ws.Cell(row, 17).Value = entry.Eigewicht;
                }
                else if (tageImBlock == 2)
                {
                    ws.Cell(row, 16).Value = "Futterverw.";
                    ws.Cell(row, 17).Style.Fill.BackgroundColor = XLColor.Yellow;
                }
                else if (tageImBlock == 3)
                {
                    ws.Cell(row, 16).Value = "W/F";
                    ws.Cell(row, 17).Value = wasserFutterRatio;
                    ws.Cell(row, 17).Style.Fill.BackgroundColor = XLColor.Yellow;
                }
                else if (tageImBlock == 4)
                {
                    ws.Cell(row, 16).Value = "Körpergew.";
                    ws.Cell(row, 17).Value = entry.Koerpergewicht;
                }
                else if (tageImBlock == 5)
                {
                    ws.Cell(row, 16).Value = "Verlust %";
                    ws.Cell(row, 17).Value = verlustProzent;
                    ws.Cell(row, 17).Style.Fill.BackgroundColor = XLColor.Yellow;
                }

                tageImBlock++;
                row++;

                // Nach 7 Tagen oder beim letzten Eintrag den Zusammenfassungs-Block (gelb) schreiben
                if (tageImBlock == 7 || i == stallEntries.Count - 1)
                {
                    int lw = (int)Math.Floor((entry.Date - stall.Einstallungsdatum).TotalDays / 7.0) + 1;
                    
                    var sumRange = ws.Range(row, 1, row, 15);
                    sumRange.Style.Fill.BackgroundColor = XLColor.Yellow;
                    sumRange.Style.Font.Bold = true;
                    sumRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    sumRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    ws.Cell(row, 1).Value = $"{lw} LW";
                    ws.Cell(row, 2).Value = "Gesamt";
                    ws.Cell(row, 3).Value = weeklyLosses;
                    ws.Cell(row, 4).Value = weeklyZugaenge;
                    ws.Cell(row, 5).Value = weeklyEggs1;
                    ws.Cell(row, 6).Value = weeklyEggs2;
                    ws.Cell(row, 7).Value = weeklyEggsTotal;
                    
                    // Durchschnittliche Legeleistung der letzten Tage
                    decimal avgLeistung = currentFlockSize > 0 ? _calcService.CalculateLayingPerformance(weeklyEggsTotal / tageImBlock, currentFlockSize) : 0;
                    ws.Cell(row, 8).Value = avgLeistung;
                    
                    ws.Cell(row, 9).Value = weeklyFutter;
                    // Futterlieferung Summe in der Woche
                    var weeklyLieferung = stallEntries.Skip(i - tageImBlock + 1).Take(tageImBlock).Sum(e => e.FutterlieferungKg ?? 0);
                    ws.Cell(row, 10).Value = weeklyLieferung;
                    
                    ws.Cell(row, 12).Value = weeklyWasser;

                    row++; // Leerzeile vor dem nächsten Block lassen, oder direkt weitermachen
                    
                    // Reset für nächsten Block
                    tageImBlock = 0;
                    blockStartRow = row;
                    weeklyLosses = 0;
                    weeklyZugaenge = 0;
                    weeklyEggs1 = 0;
                    weeklyEggs2 = 0;
                    weeklyEggsTotal = 0;
                    weeklyFutter = 0;
                    weeklyWasser = 0;
                }
            }

            // Auto-Fit columns
            ws.Columns(1, 16).AdjustToContents();
        }

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }
}
