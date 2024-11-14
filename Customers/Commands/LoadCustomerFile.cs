using crms2.Customers.Models;
using System.Data;
using System.Formats.Asn1;
using System.Globalization;

using CsvHelper;
using System.Data.Common;
using Dapper;

namespace crms2.Customers.Commands
{
    public class LoadCustomerFile
    {
        private readonly IDbConnection _dbConnection;

        public LoadCustomerFile(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<IEnumerable<CustomerModel>> ExecuteAsync(string filePath)
        {
            IEnumerable<CustomerModel> customers;

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

                // Register the custom ClassMap
                // we have a custom map because the file provided does not contain a Date field. We are generating one
                csv.Context.RegisterClassMap<CustomerMapModel>();


                // Materialize the IEnumerable to avoid ObjectDisposedException
                customers = csv.GetRecords<CustomerModel>().ToArray();
            }

            foreach (var customer in customers)
            {
                // Set CreatedAt to the current date/time since it is not provided in the CSV file
                customer.CreatedAt = DateTime.Now;

                // Insert the customer into the database
                var query = "INSERT INTO Customers (name, email, phone_number, CreatedAt) VALUES (@name, @email, @phone_number, @CreatedAt)";
                await _dbConnection.ExecuteAsync(query, customer);

                //debugging purposes
                Console.WriteLine(query);
            }

            return customers;
        }




    }
}
