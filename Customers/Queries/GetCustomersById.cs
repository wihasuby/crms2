using crms2.Customers.Models;
using Dapper;
using System.Data;

namespace crms2.Customers.Queries
{
    public class GetCustomersById
    {
        private readonly IDbConnection _dbConnection;

        public GetCustomersById(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<CustomerModel?> ExecuteAsync(long id)
        {
            string sql = "SELECT * FROM customers WHERE Id = @Id";
            return await _dbConnection.QueryFirstOrDefaultAsync<CustomerModel>(sql, new { Id = id });
        }
    }
}
