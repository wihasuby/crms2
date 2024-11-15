using Dapper;
using System.Data;
using crms2.Customers.Models;
using System.Text;

public class GetFilteredCustomers
{
    private readonly IDbConnection _dbConnection;

    public GetFilteredCustomers(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<IEnumerable<CustomerModel>> ExecuteAsync(decimal spendingThreshold)
    {


        try
        {
            // Execute the query and let Dapper handle the mapping automatically
            var result = await _dbConnection.QueryAsync<CustomerModel>(BuildQuery(spendingThreshold));
            Console.WriteLine($"Query Result Count: {result.Count()}");

            // Log each customer result for debugging
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

    public string BuildQuery(decimal st)
    {
        var sb = new StringBuilder();
        sb.AppendLine("SELECT");
        sb.AppendLine("    c.Id,");
        sb.AppendLine("    c.Name,");
        sb.AppendLine("    c.Email,");
        sb.AppendLine("    c.phone_number AS PhoneNumber,");
        sb.AppendLine("    c.CreatedAt,");
        sb.AppendLine("    COALESCE(SUM(ph.Total), 0) AS TotalSpending");
        sb.AppendLine("FROM Customers c");
        sb.AppendLine("LEFT JOIN purchase_history ph ON c.Id = ph.CustomerId");
        sb.AppendLine("GROUP BY c.Id, c.Name, c.Email, c.phone_number, c.CreatedAt");
        sb.AppendLine($"HAVING SUM(ph.Total) >= {st}");
        sb.AppendLine("ORDER BY TotalSpending ASC;");
        return sb.ToString();
    }
}
