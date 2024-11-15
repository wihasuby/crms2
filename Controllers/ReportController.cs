using crms2.Reports.Models;
using crms2.Reports.Queries;
using crms2.Customers.Queries;
using Microsoft.AspNetCore.Mvc;
using crms2.Views.Report;

namespace crms2.Controllers
{
    public class ReportController : Controller
    {
        private readonly GetMonthlyReport _getMonthlyReport;
        private readonly GetCustomerSpending _getCustomerSpending;

        public ReportController(GetMonthlyReport getMonthlyReport, GetCustomerSpending getCustomerSpending)
        {
            _getMonthlyReport = getMonthlyReport;
            _getCustomerSpending = getCustomerSpending;
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

        [HttpGet("Customers")]
        public async Task<IActionResult> Customers(string filter)
        {
            var customerData = await _getCustomerSpending.ExecuteAsync(filter);
            return View("~/Views/Report/Customers.cshtml", customerData);
        }


    }
}
