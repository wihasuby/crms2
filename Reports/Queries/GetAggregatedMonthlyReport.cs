using System.Data;
using System.Text;
using crms2.Reports.Models;
using Dapper;
using Microsoft.AspNetCore.Http.Extensions;

namespace crms2.Reports
{
    public class GetAggregatedMonthlyReport
    {
        private readonly IDbConnection _dbConnection;

        public GetAggregatedMonthlyReport(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<AggregatedMonthlyReportModel>> ExecuteAsync()
        {
            var report = await _dbConnection.QueryAsync<AggregatedMonthlyReportModel>(BuildQuery());

            return report;
        }

        public string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SELECT * FROM AggregatedMonthlyCustomerReport");

            return sb.ToString();
        }
    }
}
