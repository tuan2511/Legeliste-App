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

    public PerformanceNormService()
    {
        // Beispielhafte Norm-Daten für Lohmann Brown (vereinfacht)
        _normData = new List<PerformanceNorm>();
        
        for (int w = 18; w <= 85; w++)
        {
            decimal rate = 0;
            decimal weight = 0;
            decimal losses = 0;

            if (w == 18) rate = 2;
            else if (w == 19) rate = 15;
            else if (w == 20) rate = 48;
            else if (w == 21) rate = 82;
            else if (w == 22) rate = 92;
            else if (w >= 23 && w <= 30) rate = 95.5m - (w - 23) * 0.1m;
            else if (w > 30 && w <= 85) rate = 94.8m - (w - 30) * 0.3m;

            if (w < 20) weight = 46;
            else if (w <= 30) weight = 46 + (w - 18) * 1.2m;
            else if (w <= 85) weight = 60 + (w - 30) * 0.12m;

            losses = 0.1m + (w - 18) * 0.12m;

            _normData.Add(new PerformanceNorm { Week = w, LayingRateNorm = rate, EggWeightNorm = weight, LossRateNorm = losses });
        }
    }

    public List<PerformanceNorm> GetNormData() => _normData;

    public PerformanceNorm GetNormForWeek(int week)
    {
        return _normData.FirstOrDefault(n => n.Week == week) ?? _normData.Last();
    }
}
