
using crms2.Customers.Commands;
using crms2.PurchaseHistory.Commands;
using crms2.PurchaseHistory.Queries;
using Microsoft.Data.Sqlite;
using System.Data;

namespace crms2
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddTransient<IDbConnection>(sp => new SqliteConnection("Data Source=C:\\database\\crms.db"));

            builder.Services.AddTransient<GetAllCustomers>();
            builder.Services.AddTransient<LoadCustomer>();
            builder.Services.AddTransient<LoadCustomerFile>();
            builder.Services.AddTransient<LoadPurchaseHistoryFile>();
            builder.Services.AddTransient<GetAllPurchases>();







            var app = builder.Build();

            //seeding the database with the csv file.
            // Seed the database with the CSV file
            try
            {
                using var scope = app.Services.CreateScope();
                var seeder = scope.ServiceProvider.GetRequiredService<LoadCustomer>();
                //await seeder.LoadCustomersAsync("customers.csv");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during database seeding: {ex.Message}");
                throw;
            }

            try
            {
                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application startup failed: {ex.Message}");
                throw;
            }
        }
    }
}
