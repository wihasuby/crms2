using System.Data;
using System.Threading.Tasks;
using crms2.Customers.Models;
using Dapper;

namespace crms2.Customers.Commands
{
    public class UpdateLoyaltyPoints
    {
        private readonly IDbConnection _dbConnection;

        public UpdateLoyaltyPoints(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task ExecuteAsync(CustomerModel customer, int loyaltyPoints)
        {
            var updateQuery = "UPDATE Customers SET LoyaltyPoints = @LoyaltyPoints WHERE Id = @CustomerId";
            await _dbConnection.ExecuteAsync(updateQuery, new { LoyaltyPoints = loyaltyPoints, CustomerId = customer.Id });
        }
    }
}
