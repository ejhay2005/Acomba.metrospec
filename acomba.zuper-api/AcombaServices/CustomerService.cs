using acomba.zuper_api.Dto;
using AcoSDK;

namespace acomba.zuper_api.AcombaServices
{
    public interface ICustomerService
    {
         Task<string> AddCustomer(CustomerRequest customerRequest);
    }
    public class CustomerService : ICustomerService
    {
        private readonly IConfiguration _configuration;
        private readonly IAcombaConnection _connection;
        public CustomerService(IConfiguration configuration, IAcombaConnection connection)
        {
            _configuration = configuration;
            _connection = connection;
        }

        public async Task<string> AddCustomer(CustomerRequest customerRequest)
        {
            try
            {
                _connection.OpenConnection();
                int error;
                AcoSDK.Customer CustomerInt = new AcoSDK.Customer();
                AcoSDK.AcombaX Acomba = new AcoSDK.AcombaX();
                CustomerInt.BlankCard();
                CustomerInt.BlankKey();

                //set customer primary key
                CustomerInt.PKey_CuNumber = customerRequest.customer;

                //reserve primary key to add
                error = CustomerInt.ReserveCardNumber();

                if (error == 0)
                {
                    CustomerInt.CuNumber = CustomerInt.PKey_CuNumber;
                    CustomerInt.CuSortKey = customerRequest.customer_last_name.Substring(0, 1);
                    CustomerInt.CuName = customerRequest.customer_first_name + " " + customerRequest.customer_last_name;
                    CustomerInt.CuAddress = customerRequest.customer_address.street;
                    CustomerInt.CuCity = customerRequest.customer_address.city;
                    CustomerInt.CuPhoneNumber[(PhoneType)1] = customerRequest.customer_contact_no.mobile;
                    CustomerInt.CuActive = 1;

                    error = CustomerInt.AddCard();
                    if(error == 0)
                    {
                        return "Addition completed successfully";
                    }
                    else
                    {
                        error = CustomerInt.FreeCardNumber();
                        if (error >= 0)
                        {
                            return "Error:" + Acomba.GetErrorMessage(error);
                        }
                        else
                        {
                            return "Error:" + Acomba.GetErrorMessage(error);
                        }

                       
                    }
                    
                }
                else
                {
                    return "Error:" + Acomba.GetErrorMessage(error);
                }
            }
            catch (Exception ex)
            {
               throw;
            }
            
            
        }
    }
}
