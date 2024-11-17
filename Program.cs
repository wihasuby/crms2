using crms2.Customers.Commands;
using crms2.Customers.Queries;
using crms2.PurchaseHistory.Commands;
using crms2.PurchaseHistory.Queries;
using crms2.Reports.Queries;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
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
            builder.Services.AddRazorPages();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Get PostgreSQL connection string from configuration or environment variable
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                                 ?? Environment.GetEnvironmentVariable("CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Console.WriteLine("Error: Connection string is not set in either configuration or environment variable.");
                return;
            }

            Console.WriteLine($"Using connection string: {connectionString}");

            // Register PostgreSQL connection
            builder.Services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(connectionString));

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

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            // Map endpoints
            app.MapControllers();
            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            try
            {
                Console.WriteLine("Starting the application...");
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application startup failed: {ex.Message}");
                throw;
            }
        }
    }
}
