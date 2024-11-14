using Microsoft.AspNetCore.Mvc;
using crms2.PurchaseHistory.Queries;
using crms2.PurchaseHistory.Commands;

using crms2.Customers.Commands;

namespace crms2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchaseController : ControllerBase
    {

        private readonly GetAllPurchases _getAllPurchases;
        private readonly LoadPurchaseHistoryFile _loadPurchaseHistoryFile;


        public PurchaseController(GetAllPurchases purchases, LoadPurchaseHistoryFile loadPurchaseHistoryFile)
        { 
            _getAllPurchases = purchases;
            _loadPurchaseHistoryFile = loadPurchaseHistoryFile;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPurchases()
        {
            var purchases = await _getAllPurchases.ExecuteAsync();
            return Ok(purchases);
        }

        [HttpPost("load-from-csv")]
        public async Task<IActionResult> LoadPurchaseHistoryFromCsv([FromQuery] string filepath)
        {
            try
            {
                if (string.IsNullOrEmpty(filepath))
                    return BadRequest("File path is required.");

                var purchaseList = await _loadPurchaseHistoryFile.ExecuteAsync(filepath);
                return Ok(purchaseList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }



}
