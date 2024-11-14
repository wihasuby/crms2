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

        public CustomersController(GetAllCustomers getAllCustomers, LoadCustomer loadCustomer, LoadCustomerFile loadfile)
        {
            _getAllCustomers = getAllCustomers;
            _loadCustomer = loadCustomer;
            _loadCustomerFile = loadfile;
        }



        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _getAllCustomers.ExecuteAsync();
            return Ok(customers);
        }

        // POST: /api/customers/load-from-csv?filepath=customers.csv
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