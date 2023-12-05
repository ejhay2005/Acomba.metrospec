using acomba.zuper_api.Dto;
using AcoSDK;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace acomba.zuper_api.AcombaServices
{
    public interface ICustomerService
    {
        Task<string> AddCustomer(CustomerRequest customerRequest);
        Task<string> UpdateCustomer(CustomerRequest customerRequest);
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
                CustomerInt.PKey_CuNumber = customerRequest.customer_contact_no.phone;

                //reserve primary key to add
                error = CustomerInt.ReserveCardNumber();

                if (error == 0)
                {
                    CustomerInt.CuNumber = CustomerInt.PKey_CuNumber;
                    CustomerInt.CuSortKey = customerRequest.customer_last_name; //customerRequest.customer_last_name.Substring(0, 1);
                    CustomerInt.CuName = customerRequest.customer_first_name + " " + customerRequest.customer_last_name;
                    //CustomerInt.CuAddress = "test"; //customerRequest.customer_address.street;
                   // CustomerInt.CuCity = "test"; //customerRequest.customer_address.city;
                   // CustomerInt.CuPhoneNumber[(PhoneType)1] = "0912345678"; //customerRequest.customer_contact_no.mobile;
                    CustomerInt.CuActive = 1;

                    error = CustomerInt.AddCard();
                    if(error == 0)
                    {
                        _connection.CloseConnection();
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
        public async Task<string> UpdateCustomer(CustomerRequest customerRequest)
        {
            try
            {
                _connection.OpenConnection();
                const int noIndex = 1;
                int error,cardpos;
                AcoSDK.Customer CustomerInt = new AcoSDK.Customer();
                AcoSDK.AcombaX Acomba = new AcoSDK.AcombaX();

                CustomerInt.BlankKey();
                CustomerInt.PKey_CuNumber = customerRequest.customer_contact_no.phone;

                error = CustomerInt.FindKey(noIndex, false);
                if(error == 0)
                {
                    cardpos = CustomerInt.Key_CuCardPos;
                    error = CustomerInt.ReserveCard(cardpos);

                    if(error == 0)
                    {
                        CustomerInt.CuNumber = CustomerInt.PKey_CuNumber;
                        CustomerInt.CuSortKey = customerRequest.customer_last_name.Substring(0, 1);
                        CustomerInt.CuName = customerRequest.customer_first_name + " " + customerRequest.customer_last_name;
                        //CustomerInt.CuAddress = "test"; //customerRequest.customer_address.street;
                        // CustomerInt.CuCity = "test"; //customerRequest.customer_address.city;
                        // CustomerInt.CuPhoneNumber[(PhoneType)1] = "0912345678"; //customerRequest.customer_contact_no.mobile;
                        CustomerInt.CuActive = 1;
                        error = CustomerInt.ModifyCard(true);
                        if(error == 0)
                        {
                            return "Update completed successfully";
                        }
                        else
                        {
                            error = CustomerInt.FreeCard();
                            if(error != 0)
                            {
                                return "Error: " + Acomba.GetErrorMessage(error);
                            }
                            return "Error: " + Acomba.GetErrorMessage(error);
                        }
                    }
                    else
                    {
                        return "Error: " + Acomba.GetErrorMessage(error);
                    }
                }
                else
                {
                    return "Error :" + Acomba.GetErrorMessage(error);
                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
