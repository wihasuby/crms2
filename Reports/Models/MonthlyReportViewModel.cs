namespace crms2.Reports.Models
{
    public class MonthlyReportViewModel
    {
        public IEnumerable<MonthlyReportModel> IndividualReports { get; set; }
        public IEnumerable<AggregatedMonthlyReportModel> AggregatedReports { get; set; }

        public MonthlyReportViewModel()
        {
            IndividualReports = new List<MonthlyReportModel>();
            AggregatedReports = new List<AggregatedMonthlyReportModel>();
        }
    }
}
