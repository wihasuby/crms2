using Microsoft.AspNetCore.Mvc.RazorPages;
using crms2.Customers.Models;
using crms2.Customers.Queries;

namespace crms2.Views.Report
{
    public class CustomersModel : PageModel
    {
        public IEnumerable<CustomerModel> CustomerData { get; set; }

        public async Task OnGetAsync(string filter, GetCustomerWithFilter GetCustomerWithFilter)
        {
            CustomerData = await GetCustomerWithFilter.ExecuteAsync(filter);
        }
    }
}
