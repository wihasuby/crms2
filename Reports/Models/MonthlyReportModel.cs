namespace crms2.Reports.Models
{
    public class MonthlyReportModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal AverageSpend { get; set; }
        public int TotalLoyaltyPoints { get; set; }
    }
}
