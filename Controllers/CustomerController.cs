using crms2.Customers.Commands;
using crms2.Customers.Models;
using crms2.Customers.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;
using crms2.PurchaseHistory.Commands;

namespace crsms.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly GetAllCustomers _getAllCustomers;
        private readonly LoadCustomer _loadCustomer;
        private readonly LoadCustomerFile _loadCustomerFile;
        private readonly GetLoyaltyPoints _getLoyaltyPoints;
        private readonly GetFilteredCustomers _getFilteredCustomers;
        private readonly GetCustomerWithFilter _GetCustomerWithFilter;
        private readonly LoadPurchaseHistoryFile _loadPurchaseHistoryFile;

        public CustomersController(GetAllCustomers getAllCustomers, LoadCustomer loadCustomer, LoadCustomerFile loadfile, GetCustomerWithFilter GetCustomerWithFilter, GetFilteredCustomers getFilteredCustomers,
            LoadPurchaseHistoryFile loadPurchaseHistoryFile)
        {
            _getAllCustomers = getAllCustomers;
            _loadCustomer = loadCustomer;
            _loadCustomerFile = loadfile;
            _GetCustomerWithFilter = GetCustomerWithFilter;
            _getFilteredCustomers = getFilteredCustomers;
            _loadPurchaseHistoryFile = loadPurchaseHistoryFile;
        }

        // New API Endpoint for searching customers by name
        [HttpGet("search")]
        public async Task<IActionResult> SearchCustomers([FromQuery] string name = "")
        {
            try
            {
                Console.WriteLine($"Search request with name filter: {name}");

                // Call the GetCustomerWithFilter query to fetch filtered results
                var customers = await _GetCustomerWithFilter.ExecuteAsync(name);

                if (customers == null || !customers.Any())
                {
                    return NotFound("No customers found matching the search criteria.");
                }

                return Ok(customers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Internal server error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("filtered")]
        public async Task<IActionResult> GetFilteredCustomers([FromQuery] decimal SpendingThreshold)
        {
            try
            {
                // Adjust this method to filter by both spending and name
                var customers = await _getFilteredCustomers.ExecuteAsync(SpendingThreshold);

                if (customers == null || !customers.Any())
                {
                    return NoContent(); // Return 204 if no customers are found
                }

                return Ok(customers); // Return filtered customers
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _getAllCustomers.ExecuteAsync();
            return Ok(customers);
        }

        [HttpPost("load-file")]
        public async Task<IActionResult> LoadFile([FromQuery] string filepath)
        {
            try
            {
                if (string.IsNullOrEmpty(filepath))
                    return BadRequest("File path is required.");

                // Determine which service to call based on the file name
                if (filepath.EndsWith("customers.csv", StringComparison.OrdinalIgnoreCase))
                {
                    var customerList = await _loadCustomerFile.ExecuteAsync(filepath);
                    return Ok(customerList);
                }
                else if (filepath.EndsWith("purchase_history.csv", StringComparison.OrdinalIgnoreCase))
                {
                    var purchaseList = await _loadPurchaseHistoryFile.ExecuteAsync(filepath);
                    return Ok(purchaseList);
                }
                else
                {
                    return BadRequest("Unsupported file type. Please upload either 'customers.csv' or 'purchase_history.csv'.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateCustomer([FromBody] CustomerModel customer)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _loadCustomer.LoadCustomersAsync(customer);
            return CreatedAtAction(nameof(GetAllCustomers), customer);
        }
    }
}
