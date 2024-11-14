using crms2.Customers.Models;
using Dapper;
using System.Data;

namespace crms2.Customers.Commands
{
    public class LoadCustomer
    {

        private readonly IDbConnection _dbConnection;

        public LoadCustomer(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<int> LoadCustomersAsync(CustomerModel customer)
        {
            string sql = @"
            INSERT INTO customers (name, email, phone_number, created_at)
            VALUES (@Name, @Email, @PhoneNumber, @CreatedAt);";


            return await _dbConnection.ExecuteAsync(sql, customer);
        }
    }
}
