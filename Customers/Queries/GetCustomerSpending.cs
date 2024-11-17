using crms2.Customers.Models;
using Dapper;
using System.Data;
using System.Text;

namespace crms2.Customers.Queries
{
    public class GetCustomerWithFilter
    {
        private readonly IDbConnection _dbConnection;

        public GetCustomerWithFilter(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<CustomerModel>> ExecuteAsync(string nameFilter)
        {
            var parameters = new DynamicParameters();
            string query;

            if (string.IsNullOrEmpty(nameFilter))
            {
                // No filter, return all records
                query = BuildQueryWithoutFilter();
            }
            else
            {
                // Use filter
                parameters.Add("NameFilter", $"%{nameFilter}%");
                query = BuildQueryWithFilter();
            }

            try
            {
                var result = await _dbConnection.QueryAsync<CustomerModel>(query, parameters);
                Console.WriteLine($"Query Result Count: {result.Count()}");

                foreach (var customer in result)
                {
                    Console.WriteLine($"Customer: {customer.Name}, PhoneNumber: {customer.PhoneNumber}, TotalSpending: {customer.TotalSpending}");
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal server error: {ex.Message}");
                throw;
            }
        }


        public string BuildQueryWithoutFilter()
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT");
            sb.AppendLine("    c.Id,");
            sb.AppendLine("    c.Name,");
            sb.AppendLine("    c.Email,");
            sb.AppendLine("    c.phone_number AS PhoneNumber,");
            sb.AppendLine("    c.created_at,");
            sb.AppendLine("    COALESCE(SUM(ph.total), 0) AS TotalSpending");
            sb.AppendLine("FROM Customers c");
            sb.AppendLine("LEFT JOIN purchase_history ph ON c.Id = ph.customer_id");
            sb.AppendLine("GROUP BY c.Id");
            sb.AppendLine("ORDER BY c.Name ASC;");
            return sb.ToString();
        }

        public string BuildQueryWithFilter()
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT");
            sb.AppendLine("    c.Id,");
            sb.AppendLine("    c.Name,");
            sb.AppendLine("    c.Email,");
            sb.AppendLine("    c.phone_number AS PhoneNumber,");
            sb.AppendLine("    c.created_at,");
            sb.AppendLine("    COALESCE(SUM(ph.total), 0) AS TotalSpending");
            sb.AppendLine("FROM Customers c");
            sb.AppendLine("LEFT JOIN purchase_history ph ON c.Id = ph.customer_id");
            sb.AppendLine("WHERE c.Name ILIKE @NameFilter");
            sb.AppendLine("GROUP BY c.Id");
            sb.AppendLine("ORDER BY c.Name ASC;");
            return sb.ToString();
        }

    }
}
