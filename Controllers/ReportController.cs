using crms2.Reports.Models;
using crms2.Reports.Queries;
using Microsoft.AspNetCore.Mvc;

namespace crms2.Controllers
{
    public class ReportController : Controller
    {
        private readonly GetMonthlyReport _getMonthlyReport;

        public ReportController(GetMonthlyReport getMonthlyReport)
        {
            _getMonthlyReport = getMonthlyReport;
        }

        // API endpoint that returns JSON
        [HttpGet("api/monthly-customer-report")]
        public async Task<IEnumerable<MonthlyReportModel>> GetMonthlyCustomerReport()
        {
            var reportData = await _getMonthlyReport.ExecuteAsync();
            return reportData;
        }

        // MVC endpoint that returns an HTML view
        [HttpGet("MonthlyReport")]
        public async Task<IActionResult> MonthlyReport()
        {
            var reportData = await _getMonthlyReport.ExecuteAsync();
            return View("~/Views/Report/MonthlyReport.cshtml", reportData);
        }
    }
}
