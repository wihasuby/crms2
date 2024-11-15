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
            // Use DynamicParameters to pass the @NameFilter parameter
            var parameters = new DynamicParameters();
            parameters.Add("NameFilter", $"%{nameFilter}%");

            try
            {
                // Execute the query with parameters
                var result = await _dbConnection.QueryAsync<CustomerModel>(BuildQuery(nameFilter), parameters);
                Console.WriteLine($"Query Result Count: {result.Count()}");

                // Log the results for debugging
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

        public string BuildQuery(string filter)
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT");
            sb.AppendLine("    c.Id,");
            sb.AppendLine("    c.Name,");
            sb.AppendLine("    c.Email,");
            sb.AppendLine("    c.phone_number AS PhoneNumber,");
            sb.AppendLine("    c.CreatedAt,");
            sb.AppendLine("    SUM(ph.total) As TotalSpending");
            sb.AppendLine("FROM Customers c");
            sb.AppendLine("LEFT JOIN purchase_history ph ON c.Id = ph.CustomerId");
            sb.AppendLine($"WHERE c.Name LIKE '{filter}%'");
            sb.AppendLine("GROUP BY c.Id");
            sb.AppendLine("ORDER BY c.Name ASC;");
            return sb.ToString();
        }
    }
}
