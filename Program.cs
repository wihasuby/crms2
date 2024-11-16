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

            // Register PostgreSQL connection
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            builder.Services.AddTransient<IDbConnection>(sp => new NpgsqlConnection(connectionString));

            Console.WriteLine($"Using connection string: {connectionString}");

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

            // Initialize the PostgreSQL database
            //InitializeDatabase(connectionString);

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
                app.UseStaticFiles();
                app.UseRouting();
                app.UseAuthorization();

                app.MapControllers();
                app.MapRazorPages();
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application startup failed: {ex.Message}");
                throw;
            }
        }

        // Method to initialize the PostgreSQL database tables
        //private static void InitializeDatabase(string connectionString)
        //{
        //    using var connection = new NpgsqlConnection(connectionString);
        //    connection.Open();

        //    var createCustomersTable = @"
        //    CREATE TABLE IF NOT EXISTS customers (
        //        id SERIAL PRIMARY KEY,
        //        name TEXT NOT NULL,
        //        email TEXT UNIQUE NOT NULL,
        //        phone_number TEXT,
        //        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
        //    );";

        //    var createPurchaseHistoryTable = @"
        //    CREATE TABLE IF NOT EXISTS purchase_history (
        //        id SERIAL PRIMARY KEY,
        //        customer_id INTEGER,
        //        purchasable TEXT NOT NULL,
        //        price REAL NOT NULL,
        //        quantity INTEGER NOT NULL,
        //        total REAL GENERATED ALWAYS AS (price * quantity) STORED,
        //        purchase_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
        //        FOREIGN KEY (customer_id) REFERENCES customers(id) ON DELETE CASCADE
        //    );";


        //    try
        //    {
        //        using var command = connection.CreateCommand();

        //        // Create customers table
        //        command.CommandText = createCustomersTable;
        //        command.ExecuteNonQuery();

        //        // Create purchase_history table
        //        command.CommandText = createPurchaseHistoryTable;
        //        command.ExecuteNonQuery();

        //        Console.WriteLine("Database tables initialized successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error initializing database tables: {ex.Message}");
        //        throw;
        //    }
        }
    }
//}
