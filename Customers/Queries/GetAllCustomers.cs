using crms2.Customers.Models;
using Dapper;
using System.Data;
using System.Text;

namespace crms2.Customers.Queries
{
    public class GetAllCustomers
    {
        private readonly IDbConnection _dbConnection;

        public GetAllCustomers(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<CustomerModel>> ExecuteAsync()
        {
            return await _dbConnection.QueryAsync<CustomerModel>(BuildQuery());
        }

        public string BuildQuery()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SELECT");
            sb.AppendLine("    c.Id,");
            sb.AppendLine("    c.Name,");
            sb.AppendLine("    c.Email,");
            sb.AppendLine("    c.phone_number AS PhoneNumber,");
            sb.AppendLine("    c.created_at,");
            sb.AppendLine("    COALESCE(SUM(ph.Total), 0) AS TotalSpending");
            sb.AppendLine("FROM Customers c");
            sb.AppendLine("LEFT JOIN purchase_history ph ON c.Id = ph.customer_id");
            sb.AppendLine("GROUP BY c.Id, c.Name, c.Email, c.phone_number, c.created_at");
            sb.AppendLine("ORDER BY TotalSpending ASC;");


            return sb.ToString();

        }

    }
}


