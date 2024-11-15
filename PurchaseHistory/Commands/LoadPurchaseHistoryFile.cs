using crms2.PurchaseHistory.Models;
using crms2.Customers.Models;
using System.Data;
using System.Globalization;
using CsvHelper;
using Dapper;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace crms2.PurchaseHistory.Commands
{
    public class LoadPurchaseHistoryFile
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<LoadPurchaseHistoryFile> _logger;

        public LoadPurchaseHistoryFile(IDbConnection dbConnection, ILogger<LoadPurchaseHistoryFile> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public async Task<IEnumerable<PurchaseHistoryModel>> ExecuteAsync(string filePath)
        {
            IEnumerable<PurchaseHistoryModel> purchases;
            var validPurchases = new List<PurchaseHistoryModel>();
            var invalidPurchases = new List<PurchaseHistoryModel>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Register the custom ClassMap for PurchaseHistoryModel
                csv.Context.RegisterClassMap<PurchaseHistoryMapModel>();
                purchases = csv.GetRecords<PurchaseHistoryModel>().ToArray();
            }

            // emails need to be lowercase Requirement 2.1
            var customerEmails = await _dbConnection.QueryAsync<string>("SELECT LOWER(Email) FROM Customers");
            var customerEmailSet = new HashSet<string>(customerEmails);

            foreach (var purchase in purchases)
            {
                try
                {
                    // Validate the purchase record using data annotations
                    ValidatePurchase(purchase);

                    // Check if the customer email exists in the Customers table
                    if (string.IsNullOrEmpty(purchase.CustomerEmail) ||
                        !customerEmailSet.Contains(purchase.CustomerEmail.ToLower()))
                    {
                        _logger.LogInformation($"Customer email {purchase.CustomerEmail} not found. Skipping record.");
                        invalidPurchases.Add(purchase);
                        continue;
                    }

                    // Lookup CustomerId using the customer's email
                    var customerQuery = "SELECT Id FROM Customers WHERE LOWER(Email) = LOWER(@Email)";
                    var customerId = await _dbConnection.QuerySingleOrDefaultAsync<int>(customerQuery, new { Email = purchase.CustomerEmail });

                    if (customerId == 0)
                    {
                        _logger.LogInformation($"Customer with email {purchase.CustomerEmail} not found. Skipping record.");
                        invalidPurchases.Add(purchase);
                        continue;
                    }

                    // Set the CustomerId
                    purchase.CustomerId = customerId;

                    // Add valid purchase to the list
                    validPurchases.Add(purchase);
                }
                catch (ValidationException ex)
                {
                    _logger.LogInformation($"Validation error for purchase: {ex.Message}");
                    invalidPurchases.Add(purchase);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Unexpected error for purchase: {ex.Message}");
                    invalidPurchases.Add(purchase);
                }
            }

            // Insert all valid purchases into the database
            try
            {
                if (validPurchases.Any())
                {
                    var insertQuery = @"
                        INSERT INTO purchase_history (CustomerId, Purchasable, Price, Quantity, PurchaseDate)
                        VALUES (@CustomerId, @Purchasable, @Price, @Quantity, @PurchaseDate)";
                    await _dbConnection.ExecuteAsync(insertQuery, validPurchases);
                    _logger.LogInformation($"Successfully inserted {validPurchases.Count} purchase records.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Database insertion error: {ex.Message}");
                throw;
            }

            return validPurchases;
        }

        // Method to manually validate the PurchaseHistoryModel
        private void ValidatePurchase(PurchaseHistoryModel purchase)
        {
            var context = new ValidationContext(purchase);
            var results = new List<ValidationResult>();

            // Perform validation
            bool isValid = Validator.TryValidateObject(purchase, context, results, true);

            // If the model is invalid, throw a ValidationException
            if (!isValid)
            {
                var errorMessages = string.Join("; ", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"Purchase validation failed: {errorMessages}");
            }
        }
    }
}
