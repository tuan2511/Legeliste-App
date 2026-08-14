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

        var stalls = entries.Select(e => e.Stall).DistinctBy(s => s?.Id).ToList();

        // Moderne Farben
        var headerBg = XLColor.FromHtml("#1e3a8a"); // bg-blue-900
        var headerFg = XLColor.White;
        var rowBgAlt = XLColor.FromHtml("#f8fafc"); // bg-slate-50
        var rowBg = XLColor.White;
        var summaryBg = XLColor.FromHtml("#d1fae5"); // bg-emerald-100 (green accent)

        foreach (var stall in stalls)
        {
            if (stall == null) continue;

            var stallEntries = entries.Where(e => e.StallId == stall.Id).OrderBy(e => e.Date).ToList();
            if (!stallEntries.Any()) continue;

            var ws = workbook.Worksheets.Add(stall.Name.Length > 31 ? stall.Name.Substring(0, 31) : stall.Name);

            // Setup Header Row
            string[] headers = { 
                "Datum", "Hühner", "Verluste", "Zugänge", "Eier 1e Wahl", "Eier 2e Wahl", 
                "Eier Gesamt", "Leistung %", "Futter kg", "Lieferung kg", "Futter g/Tier", 
                "Wasser L", "Wasser ml/Tier", "Licht", "Auslauf", 
                "Kontrolle", "Bemerkungen" 
            };
            
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Fill.BackgroundColor = headerBg;
                cell.Style.Font.FontColor = headerFg;
                cell.Style.Font.Bold = true;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#0f172a");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            int row = 2;
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
                
                var pastChanges = await context.DailyEntries
                    .Where(e => e.StallId == entry.StallId && e.Date < entry.Date && e.Status == EntryStatus.Freigegeben)
                    .SumAsync(e => (e.Verluste ?? 0) - (e.ZugaengeTiere ?? 0));
                
                int currentFlockSize = stall.AnfangsbestandTiere - pastChanges;
                int todayLosses = entry.Verluste ?? 0;
                int todayZugaenge = entry.ZugaengeTiere ?? 0;
                int totalEggs = (entry.Eier1Wahl ?? 0) + (entry.Eier2Wahl ?? 0);
                
                var legeleistung = _calcService.CalculateLayingPerformance(totalEggs, currentFlockSize);
                var futterProTier = _calcService.CalculateFeedPerBird(entry.FutterKg ?? 0, currentFlockSize);
                var wasserProTier = _calcService.CalculateWaterPerBird(entry.WasserLiter ?? 0, currentFlockSize);

                // Alternierende Zeilenfarbe
                var currentBg = (row % 2 == 0) ? rowBgAlt : rowBg;

                var dataRow = ws.Range(row, 1, row, headers.Length);
                dataRow.Style.Fill.BackgroundColor = currentBg;
                dataRow.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                dataRow.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRow.Style.Border.InsideBorderColor = XLColor.FromHtml("#e2e8f0");
                dataRow.Style.Border.OutsideBorderColor = XLColor.FromHtml("#cbd5e1");

                ws.Cell(row, 1).Value = entry.Date.ToShortDateString();
                ws.Cell(row, 2).Value = currentFlockSize;
                ws.Cell(row, 3).Value = todayLosses;
                ws.Cell(row, 4).Value = todayZugaenge;
                ws.Cell(row, 5).Value = entry.Eier1Wahl;
                ws.Cell(row, 6).Value = entry.Eier2Wahl;
                ws.Cell(row, 7).Value = totalEggs;
                
                ws.Cell(row, 8).Value = legeleistung;
                ws.Cell(row, 8).Style.NumberFormat.Format = "0.00";
                
                ws.Cell(row, 9).Value = entry.FutterKg;
                ws.Cell(row, 10).Value = entry.FutterlieferungKg;
                ws.Cell(row, 11).Value = futterProTier;
                ws.Cell(row, 12).Value = entry.WasserLiter;
                ws.Cell(row, 13).Value = wasserProTier;
                
                ws.Cell(row, 14).Value = $"{entry.LichtVon}-{entry.LichtBis}";
                ws.Cell(row, 15).Value = $"M: {entry.AuslaufzeitMorgensVon}-{entry.AuslaufzeitMorgensBis}\nA: {entry.AuslaufzeitAbendsVon}-{entry.AuslaufzeitAbendsBis}";
                ws.Cell(row, 15).Style.Alignment.WrapText = true;
                
                ws.Cell(row, 16).Value = $"{entry.KontrollzeitenVon}-{entry.KontrollzeitenBis}";
                ws.Cell(row, 17).Value = entry.Bemerkungen;

                weeklyLosses += todayLosses;
                weeklyZugaenge += todayZugaenge;
                weeklyEggs1 += entry.Eier1Wahl ?? 0;
                weeklyEggs2 += entry.Eier2Wahl ?? 0;
                weeklyEggsTotal += totalEggs;
                weeklyFutter += entry.FutterKg ?? 0;
                weeklyWasser += entry.WasserLiter ?? 0;
                
                tageImBlock++;
                row++;

                if (tageImBlock == 7 || i == stallEntries.Count - 1)
                {
                    int lw = (int)Math.Floor((entry.Date - stall.Einstallungsdatum).TotalDays / 7.0) + 1;
                    
                    var sumRange = ws.Range(row, 1, row, headers.Length);
                    sumRange.Style.Fill.BackgroundColor = summaryBg;
                    sumRange.Style.Font.Bold = true;
                    sumRange.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    sumRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#059669"); // emerald-600

                    ws.Cell(row, 1).Value = $"{lw} LW (Summe)";
                    ws.Cell(row, 2).Value = "-";
                    ws.Cell(row, 3).Value = weeklyLosses;
                    ws.Cell(row, 4).Value = weeklyZugaenge;
                    ws.Cell(row, 5).Value = weeklyEggs1;
                    ws.Cell(row, 6).Value = weeklyEggs2;
                    ws.Cell(row, 7).Value = weeklyEggsTotal;
                    
                    decimal avgLeistung = currentFlockSize > 0 ? _calcService.CalculateLayingPerformance(weeklyEggsTotal / tageImBlock, currentFlockSize) : 0;
                    ws.Cell(row, 8).Value = avgLeistung;
                    ws.Cell(row, 8).Style.NumberFormat.Format = "0.00";
                    
                    ws.Cell(row, 9).Value = weeklyFutter;
                    var weeklyLieferung = stallEntries.Skip(i - tageImBlock + 1).Take(tageImBlock).Sum(e => e.FutterlieferungKg ?? 0);
                    ws.Cell(row, 10).Value = weeklyLieferung;
                    ws.Cell(row, 12).Value = weeklyWasser;

                    row += 2; // Leerzeile vor nächstem Block
                    tageImBlock = 0;
                    weeklyLosses = 0;
                    weeklyZugaenge = 0;
                    weeklyEggs1 = 0;
                    weeklyEggs2 = 0;
                    weeklyEggsTotal = 0;
                    weeklyFutter = 0;
                    weeklyWasser = 0;
                }
            }

            ws.Columns(1, headers.Length).AdjustToContents();
            ws.Column(15).Width = 15; // Auslauf
            ws.Column(17).Width = 30; // Bemerkungen
        }

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }
}
