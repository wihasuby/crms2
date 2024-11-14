using crms2.PurchaseHistory.Models;
using crms2.Customers.Models;
using System.Data;
using System.Globalization;
using CsvHelper;
using Dapper;
using crms2.PurchaseHistory.Models;

namespace crms2.PurchaseHistory.Commands
{
    public class LoadPurchaseHistoryFile
    {
        private readonly IDbConnection _dbConnection;

        public LoadPurchaseHistoryFile(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<PurchaseHistoryModel>> ExecuteAsync(string filePath)
        {
            IEnumerable<PurchaseHistoryModel> purchases;

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Register the custom ClassMap for PurchaseHistoryModel
                csv.Context.RegisterClassMap<PurchaseHistoryMapModel>();
                purchases = csv.GetRecords<PurchaseHistoryModel>().ToArray();
            }

            // Get all customer emails from the database (case-insensitive)
            var customerEmails = await _dbConnection.QueryAsync<string>("SELECT LOWER(Email) FROM Customers");
            var customerEmailSet = new HashSet<string>(customerEmails);

            foreach (var purchase in purchases)
            {
                try
                {
                    // Check if the customer email exists in the Customers table

                    if (string.IsNullOrEmpty(purchase.CustomerEmail) ||
                        !customerEmailSet.Contains(purchase.CustomerEmail.ToLower()))
                    {
                        Console.WriteLine($"Customer email {purchase.CustomerEmail} not found. Skipping record.");
                        continue;
                    }

                    // Lookup CustomerId using the customer's email
                    // Requirement 2.4 "Ensure purchase records link to the corresponding customer based on
                    // the customer_email column in the CSV file and the customer_id column in the database."
                    var customerQuery = "SELECT Id FROM Customers WHERE LOWER(Email) = LOWER(@Email)";
                    var customerId = await _dbConnection.QuerySingleOrDefaultAsync<int>(customerQuery, new { Email = purchase.CustomerEmail });

                    if (customerId == 0)
                    {
                        Console.WriteLine($"Customer with email {purchase.CustomerEmail} not found. Skipping record.");
                        continue;
                    }

                    // Set the CustomerId
                    purchase.CustomerId = customerId;

                    // Insert the purchase record into the database
                    var insertQuery = @"
                        INSERT INTO purchase_history (CustomerId, Purchasable, Price, Quantity, PurchaseDate)
                        VALUES (@CustomerId, @Purchasable, @Price, @Quantity, @PurchaseDate)";

                    var result = await _dbConnection.ExecuteAsync(insertQuery, purchase);

                    if (result == 0)
                    {
                        Console.WriteLine($"Failed to insert purchase record for {purchase.Purchasable}.");
                    }
                    else
                    {
                        Console.WriteLine($"Successfully inserted purchase record for {purchase.Purchasable}.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting purchase record: {ex.Message}");
                }
            }

            return purchases;
        }
    }
}
