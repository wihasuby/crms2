using CsvHelper.Configuration;
using crms2.PurchaseHistory.Models;

namespace crms2.PurchaseHistory.Models
{
    public class PurchaseHistoryMapModel : ClassMap<PurchaseHistoryModel>
    {
        public PurchaseHistoryMapModel()
        {
            Map(m => m.CustomerEmail).Name("customer_email");
            Map(m => m.Purchasable).Name("purchasable");
            Map(m => m.Price).Name("price");
            Map(m => m.Quantity).Name("quantity");
            Map(m => m.PurchaseDate).Name("date");
        }
    }
}
