using crms2.Customers.Commands;
using crms2.Customers.Models;
using crms2.Customers.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Dapper;

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
        private readonly GetCustomerSpending _getCustomerSpending;

        public CustomersController(
            GetAllCustomers getAllCustomers,
            LoadCustomer loadCustomer,
            LoadCustomerFile loadfile,
            GetCustomerSpending getCustomerSpending,
            GetFilteredCustomers getFilteredCustomers)
        {
            _getAllCustomers = getAllCustomers;
            _loadCustomer = loadCustomer;
            _loadCustomerFile = loadfile;
            _getCustomerSpending = getCustomerSpending;
            _getFilteredCustomers = getFilteredCustomers;
        }

        // New API Endpoint for searching customers by name
        [HttpGet("search")]
        public async Task<IActionResult> SearchCustomers([FromQuery] string name = "")
        {
            try
            {
                Console.WriteLine($"Search request with name filter: {name}");

                // Call the GetCustomerSpending query to fetch filtered results
                var customers = await _getCustomerSpending.ExecuteAsync(name);

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
                var customers = await _getFilteredCustomers.ExecuteAsync(SpendingThreshold);
                return Ok(customers);
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

        [HttpPost("load-from-csv")]
        public async Task<IActionResult> LoadCustomersFromCsv([FromQuery] string filepath)
        {
            try
            {
                if (string.IsNullOrEmpty(filepath))
                    return BadRequest("File path is required.");

                var customerList = await _loadCustomerFile.ExecuteAsync(filepath);
                return Ok(customerList);
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
