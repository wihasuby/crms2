namespace crms2.Reports.Models
{
    public class AggregatedMonthlyReportModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalAverageSpend { get; set; }
        public int TotalLoyaltyPoints { get; set; }
    }
}
