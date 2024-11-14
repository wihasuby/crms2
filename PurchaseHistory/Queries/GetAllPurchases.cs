using crms2.Customers.Models;
using crms2.PurchaseHistory.Models;
using System.Data;
using System.Data.Common;
using Dapper;

namespace crms2.PurchaseHistory.Queries
{
    public class GetAllPurchases
    {
        private readonly IDbConnection _dbconnection; 

        public GetAllPurchases(IDbConnection dbconnection)
        {
            _dbconnection = dbconnection;
        }

        public async Task<IEnumerable<PurchaseHistoryModel>> ExecuteAsync()
        {
            string sql = "SELECT * FROM customers";
            return await _dbconnection.QueryAsync<PurchaseHistoryModel>(sql);
        }
    }
}
