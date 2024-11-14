using CsvHelper.Configuration;
using crms2.Customers.Models;

public class CustomerMapModel : ClassMap<CustomerModel>
{
    public CustomerMapModel()
    {
        Map(m => m.name).Name("name");
        Map(m => m.email).Name("email");
        Map(m => m.phone_number).Name("phone_number");
        // Do not map the CreatedAt property; it will be set automatically
    }
}
