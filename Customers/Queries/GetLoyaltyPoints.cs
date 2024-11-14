using System;
using System.Data;
using System.Threading.Tasks;
using crms2.Customers.Models;
using crms2.PurchaseHistory.Models;
using Dapper;

namespace crms2.Customers.Queries
{
    public class GetLoyaltyPoints
    {
        private readonly IDbConnection _dbConnection;

        public GetLoyaltyPoints(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<CustomerModel>> ExecuteAsync()
        {
            // Step 1: Get all customers from the database
            var customers = await _dbConnection.QueryAsync<CustomerModel>("SELECT * FROM Customers");

            foreach (var cust in customers)
            {
                //  Only get purchases that have a purchase date > 22-01-01. Requirement 4.3
                var purchasesQuery = @"
                    SELECT *
                    FROM purchase_history
                    WHERE CustomerId = @CustomerId
                      AND PurchaseDate >= '2022-01-01'";

                var purchases = await _dbConnection.QueryAsync<PurchaseHistoryModel>(purchasesQuery, new { CustomerId = cust.Id });
                int purchaseCounter = 0;

                if (cust.Id == 16)
                {
                    Console.WriteLine("test");
                }
               
                //Calculation of loyalty points. 

                // For every $10 spent, a customer earns 1 loyalty point. This line sums up all their purchase totals from the purchase_history table to get
                // a master total. That number divided by 10 is their loyalty point accrual. 
                decimal totalCust = purchases.Sum(p => p.Total) / 10;

                //For every 10 purchases, a customer is rewarded 10 more loyalty points. This line gets a sum of all their Orders from the purchase history, 
                //If the quanity total modulo 10 is 0, just add the total. If modulo 10 is not 0, divide it and then multiply by 10 to get their rewards. 

                /*
                 * Example: 
                 * Customer 16 Has a total of 2932.23 dollars spent. This yields 292 loyalty points
                 * Customer 16 also has 59 total orders. That would be 50 extra loyalty points.
                 */ 
                int totalQuant = purchases.Sum(p => p.Quantity) / 10;

                if(totalQuant % 10 == 0)
                {
                    cust.LoyaltyPoints = (int)totalCust + totalQuant;
                }
                else
                {
                    cust.LoyaltyPoints = (int)totalCust + totalQuant * 10;

                }




            }
            return customers;
        }
    }
}
