using crms2.Customers.Models;
using System.Data;
using System.Globalization;
using CsvHelper;
using Dapper;
using System.ComponentModel.DataAnnotations;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace crms2.Customers.Commands
{
    public class LoadCustomerFile
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<LoadCustomerFile> _logger;


        public LoadCustomerFile(IDbConnection dbConnection, ILogger<LoadCustomerFile> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;   
        }

        public async Task<IEnumerable<CustomerModel>> ExecuteAsync(string filePath)
        {
            IEnumerable<CustomerModel> inputData;
            var validCustomers = new List<CustomerModel>();
            var invalidCustomers = new List<CustomerModel>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Register the custom ClassMap
                csv.Context.RegisterClassMap<CustomerMapModel>();

                // Materialize the IEnumerable to avoid ObjectDisposedException
                inputData = csv.GetRecords<CustomerModel>().ToArray();
            }

            foreach (var customer in inputData)
            {
                try
                {
                    // Set CreatedAt to the current date/time since it is not provided in the CSV file
                    customer.CreatedAt = DateTime.Now;

                    // Manually trigger validation
                    ValidateCustomer(customer);

                    // Add valid customer to the list
                    validCustomers.Add(customer);
                }
                catch (ValidationException ex)
                {
                    // Add invalid customer to the list and log the error
                    invalidCustomers.Add(customer);
                    _logger.LogInformation($"Validation error for {customer.Name ?? "Unknown"}: {ex.Message}");
                }
            }

            // Insert all valid customers into the database
            try
            {
                if (validCustomers.Any())
                {
                    var query = "INSERT INTO Customers (name, email, phone_number, created_at) VALUES (@Name, @Email, @PhoneNumber, @CreatedAt)";

                    await _dbConnection.ExecuteAsync(query, validCustomers);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Database insertion error: {ex.Message}");
                throw;
            }

            // Return both valid and invalid customers for reporting
            return validCustomers;
        }

        // Method to manually validate the CustomerModel
        private void ValidateCustomer(CustomerModel customer)
        {
            var context = new ValidationContext(customer);
            var results = new List<ValidationResult>();

            // Perform validation
            bool isValid = Validator.TryValidateObject(customer, context, results, true);

            // If the model is invalid, throw a ValidationException
            if (!isValid)
            {
                var errorMessages = string.Join("; ", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"Customer validation failed: {customer.Name ?? "Unknown"} - {errorMessages}");
            }
        }
    }
}
