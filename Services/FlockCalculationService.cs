namespace LegelisteApp.Services;

public class FlockCalculationService
{
    public string GetFlockAge(DateTime einstellungsdatum, DateTime currentDate)
    {
        var age = currentDate.Date - einstellungsdatum.Date;
        if (age.TotalDays < 0) return "0W 0T";

        int weeks = (int)(age.TotalDays / 7);
        int days = (int)(age.TotalDays % 7);

        return $"{weeks}W {days}T";
    }

    public decimal CalculateLayingPerformance(int totalEggs, int currentFlockSize)
    {
        if (currentFlockSize <= 0) return 0;
        return ((decimal)totalEggs / currentFlockSize) * 100;
    }

    public decimal CalculateFeedPerBird(decimal futterKg, int currentFlockSize)
    {
        if (currentFlockSize <= 0) return 0;
        return (futterKg * 1000) / currentFlockSize;
    }

    public decimal CalculateWaterPerBird(decimal wasserLiter, int currentFlockSize)
    {
        if (currentFlockSize <= 0) return 0;
        return (wasserLiter * 1000) / currentFlockSize;
    }

    public decimal CalculateWaterFeedRatio(decimal wasserLiter, decimal futterKg)
    {
        if (futterKg <= 0) return 0;
        return wasserLiter / futterKg;
    }
}
