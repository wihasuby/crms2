using CsvHelper.Configuration;
using crms2.Customers.Models;

public class CustomerMapModel : ClassMap<CustomerModel>
{
    public CustomerMapModel()
    {
        Map(m => m.Name).Name("name");
        Map(m => m.Email).Name("email");
        Map(m => m.PhoneNumber).Name("phone_number");
        // Do not map the CreatedAt property; it will be set automatically
    }
}
