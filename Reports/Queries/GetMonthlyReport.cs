using System.Data;
using System.Text;
using crms2.Reports.Models;
using Dapper;
using Microsoft.AspNetCore.Http.Extensions;

namespace crms2.Reports.Queries
{
    public class GetMonthlyReport
    {
        private readonly IDbConnection _dbConnection;

        public GetMonthlyReport (IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }       

        public async Task<IEnumerable<MonthlyReportModel>> ExecuteAsync()
        {
            var report = await _dbConnection.QueryAsync<MonthlyReportModel>(BuildQuery());

            return report;
        }

        public string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SELECT * FROM MonthlyCustomerReport");

            return sb.ToString();
        }
    }
}
