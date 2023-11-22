using acomba.zuper_api.Dto;

namespace acomba.zuper_api.AcombaServices
{
    public interface IinvoiceService
    {
        Task<string> AddInvoice(InvoiceRequest invoiceRequest);
    }
    public class InvoiceService : IinvoiceService
    {
        private readonly IConfiguration _configuration;
        private readonly IAcombaConnection _connection;
        public CustomerService(IConfiguration configuration, IAcombaConnection connection)
        {
            _configuration = configuration;
            _connection = connection;
        }

        public async Task<string> AddInvoice(InvoiceRequest invoiceRequest)
        {
            try
            {
                _connection.OpenConnection();
                int error;

            }
            catch(Exception ex) 
            {
                throw;
            }
        }
    }
}
