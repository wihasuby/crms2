using crms2.Customers.Models;
using Dapper;
using System.Data;

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
            string sql = "SELECT * FROM customers";
            return await _dbConnection.QueryAsync<CustomerModel>(sql);
        }

    }
}


