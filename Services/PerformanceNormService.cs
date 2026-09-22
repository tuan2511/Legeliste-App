using LegelisteApp.Data.Models;

namespace LegelisteApp.Services;

public class PerformanceNormService
{
    public class PerformanceNorm
    {
        public int Week { get; set; }
        public decimal LayingRateNorm { get; set; } // %
        public decimal EggWeightNorm { get; set; } // g
        public decimal LossRateNorm { get; set; } // cumulative %
    }

    private readonly List<PerformanceNorm> _normData;

    // Quelle: LOHMANN Management Guide, "Performance Data / Table 18: Performance Goals of
    // LOHMANN BROWN-CLASSIC" (Rate of Lay % per Hen-Day, Egg Weight g je Woche).
    // https://lohmann-breeders.com/files/downloads/MG/e-Guides/Cage/English/LB_eMG_Cage_EN_PerfData_LB-Classic_p8.pdf
    private static readonly Dictionary<int, (decimal Rate, decimal Weight)> OfficialData = new()
    {
        { 19, (9.0m, 43.6m) }, { 20, (36.4m, 46.1m) }, { 21, (54.4m, 48.7m) }, { 22, (71.9m, 51.1m) },
        { 23, (82.3m, 53.3m) }, { 24, (87.9m, 55.3m) }, { 25, (91.1m, 57.0m) }, { 26, (92.9m, 58.2m) },
        { 27, (94.0m, 59.3m) }, { 28, (94.6m, 60.2m) }, { 29, (94.9m, 61.0m) }, { 30, (95.1m, 61.6m) },
        { 31, (95.3m, 62.1m) }, { 32, (95.4m, 62.5m) }, { 33, (95.5m, 62.9m) }, { 34, (95.5m, 63.3m) },
        { 35, (95.4m, 63.7m) }, { 36, (95.3m, 63.9m) }, { 37, (95.1m, 64.1m) }, { 38, (94.9m, 64.3m) },
        { 39, (94.8m, 64.4m) }, { 40, (94.6m, 64.6m) }, { 41, (94.4m, 64.7m) }, { 42, (94.2m, 64.9m) },
        { 43, (94.0m, 65.0m) }, { 44, (93.8m, 65.1m) }, { 45, (93.5m, 65.2m) }, { 46, (93.2m, 65.3m) },
        { 47, (92.9m, 65.4m) }, { 48, (92.6m, 65.5m) }, { 49, (92.3m, 65.6m) }, { 50, (92.0m, 65.7m) },
        { 51, (91.7m, 65.8m) }, { 52, (91.3m, 65.9m) }, { 53, (91.0m, 66.0m) }, { 54, (90.6m, 66.1m) },
        { 55, (90.3m, 66.1m) }, { 56, (89.9m, 66.2m) }, { 57, (89.5m, 66.2m) }, { 58, (89.2m, 66.3m) },
        { 59, (88.8m, 66.3m) }, { 60, (88.4m, 66.4m) }, { 61, (88.1m, 66.4m) }, { 62, (87.7m, 66.5m) },
        { 63, (87.3m, 66.6m) }, { 64, (86.9m, 66.6m) }, { 65, (86.5m, 66.7m) }, { 66, (86.1m, 66.8m) },
        { 67, (85.7m, 66.8m) }, { 68, (85.3m, 66.9m) }, { 69, (84.9m, 66.9m) }, { 70, (84.4m, 67.0m) },
        { 71, (83.9m, 67.0m) }, { 72, (83.4m, 67.1m) }, { 73, (82.9m, 67.1m) }, { 74, (82.4m, 67.2m) },
        { 75, (81.9m, 67.2m) }, { 76, (81.3m, 67.3m) }, { 77, (80.8m, 67.3m) }, { 78, (80.3m, 67.4m) },
        { 79, (79.7m, 67.4m) }, { 80, (79.2m, 67.5m) }, { 81, (78.6m, 67.5m) }, { 82, (78.0m, 67.6m) },
        { 83, (77.4m, 67.6m) }, { 84, (76.8m, 67.6m) }, { 85, (76.3m, 67.7m) },
    };

    // Liveability-Ankerpunkte aus dem Management Guide: Aufzucht 98-99%, Legeperiode
    // 72 Wochen 95-96%, 100 Wochen 90-91% -> Mittelwerte als kumulierte Verlust-%-Kurve.
    private static readonly (int Week, decimal LossPct)[] LossAnchors =
    {
        (18, 1.5m), (72, 4.5m), (100, 9.5m)
    };

    public PerformanceNormService()
    {
        _normData = new List<PerformanceNorm>();

        for (int w = 18; w <= 85; w++)
        {
            decimal rate;
            decimal weight;

            if (w == 18)
            {
                // Vor Legebeginn: kein offizieller Wert, Anschluss an Woche 19.
                rate = 2m;
                weight = 42m;
            }
            else
            {
                var data = OfficialData[w];
                rate = data.Rate;
                weight = data.Weight;
            }

            var losses = InterpolateLoss(w);

            _normData.Add(new PerformanceNorm { Week = w, LayingRateNorm = rate, EggWeightNorm = weight, LossRateNorm = losses });
        }
    }

    private static decimal InterpolateLoss(int week)
    {
        for (int i = 0; i < LossAnchors.Length - 1; i++)
        {
            var (w1, l1) = LossAnchors[i];
            var (w2, l2) = LossAnchors[i + 1];
            if (week >= w1 && week <= w2)
            {
                var fraction = (decimal)(week - w1) / (w2 - w1);
                return Math.Round(l1 + fraction * (l2 - l1), 2);
            }
        }
        return LossAnchors[^1].LossPct;
    }

    public List<PerformanceNorm> GetNormData() => _normData;

    public PerformanceNorm GetNormForWeek(int week)
    {
        return _normData.FirstOrDefault(n => n.Week == week) ?? _normData.Last();
    }
}
