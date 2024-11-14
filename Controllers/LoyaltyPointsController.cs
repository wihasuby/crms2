using crms2.Customers.Models;
using crms2.Customers.Queries;
using Microsoft.AspNetCore.Mvc;

namespace crms2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoyaltyPointsController : ControllerBase
    {
        private readonly GetLoyaltyPoints _getLoyaltyPoints;

        public LoyaltyPointsController (GetLoyaltyPoints loyaltyPoints)
        {
            _getLoyaltyPoints = loyaltyPoints;
        }

        [HttpGet]
        public async Task<IEnumerable<CustomerModel>> CalculateLoyaltyPoints()
        {
            var customers = await _getLoyaltyPoints.ExecuteAsync();
            return customers;
        }
    }
}
