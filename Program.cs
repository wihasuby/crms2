using crms2.Customers.Commands;
using crms2.Customers.Queries;
using crms2.PurchaseHistory.Commands;
using crms2.PurchaseHistory.Queries;
using crms2.Reports.Queries;
using Microsoft.Data.Sqlite;
using System.Data;

namespace crms2
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages(); // Enable Razor Pages
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register SQLite connection
            builder.Services.AddTransient<IDbConnection>(sp => new SqliteConnection("Data Source=C:\\database\\crms.db"));

            // Register application services
            builder.Services.AddTransient<GetAllCustomers>();
            builder.Services.AddTransient<GetCustomerWithFilter>();
            builder.Services.AddTransient<GetMonthlyReport>();
            builder.Services.AddTransient<GetLoyaltyPoints>();
            builder.Services.AddTransient<LoadCustomer>();
            builder.Services.AddTransient<LoadCustomerFile>();
            builder.Services.AddTransient<LoadPurchaseHistoryFile>();
            builder.Services.AddTransient<GetAllPurchases>();
            builder.Services.AddTransient<GetFilteredCustomers>();

            var app = builder.Build();

            // Seed the database with the CSV file
            try
            {
                using var scope = app.Services.CreateScope();
                var seeder = scope.ServiceProvider.GetRequiredService<LoadCustomer>();
                // Uncomment the line below to seed data from the CSV file
                // await seeder.LoadCustomersAsync("customers.csv");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during database seeding: {ex.Message}");
                throw;
            }

            try
            {
                // Configure the HTTP request pipeline
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles(); // Serve static files (e.g., CSS, JS)
                app.UseRouting();
                app.UseAuthorization();

                // Map controllers and Razor Pages
                app.MapControllers();
                app.MapRazorPages(); // Ensure this line is present
                app.MapDefaultControllerRoute();

                // Run the application
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


