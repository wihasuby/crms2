using System;
using System.Data;
using System.Threading.Tasks;
using crms2.Customers.Models;
using crms2.Customers.Queries;
using crms2.Customers.Commands;
using Dapper;

namespace crms2.Customers.Services
{
    public class LoyaltyPointsService
    {
        private readonly GetLoyaltyPoints _loyaltyPointsQuery;
        private readonly UpdateLoyaltyPoints _updateLoyaltyPointsCommand;

        public LoyaltyPointsService(GetLoyaltyPoints loyaltyPointsQuery, UpdateLoyaltyPoints updateLoyaltyPointsCommand)
        {
            _loyaltyPointsQuery = loyaltyPointsQuery;
            _updateLoyaltyPointsCommand = updateLoyaltyPointsCommand;
        }

        public async Task ExecuteAsync(IDbConnection dbConnection)
        {
            // Get all customers
            var customers = await dbConnection.QueryAsync<CustomerModel>("SELECT * FROM Customers");

            //foreach (var customer in customers)
            //{
            //    // Calculate loyalty points
            //   //(CustomerModel cust, int loyaltyPoints) = await _loyaltyPointsQuery.ExecuteAsync();

            //    // Update loyalty points in the database
            //    await _updateLoyaltyPointsCommand.ExecuteAsync(cust, loyaltyPoints);

            //    Console.WriteLine($"Updated CustomerId {customer.Id} with {loyaltyPoints} loyalty points.");
            //}
        }
    }
}
