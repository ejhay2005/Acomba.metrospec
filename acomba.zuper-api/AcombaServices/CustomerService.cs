using acomba.zuper_api.Dto;
using AcoSDK;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using System.Text;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace acomba.zuper_api.AcombaServices
{
    public interface ICustomerService
    {
        Task<string> AddCustomer(CustomerRequest customerRequest);
        Task<string> AddCustomerWebhook(CustomerRequestDto customerRequest);
        Task<string> UpdateCustomer(CustomerRequest customerRequest);
        Task<List<string>> ImportCustomers(List<CustomerDto> customers);
        Task<object> ImportCustomersToZuper();
    }
    public class CustomerService : ICustomerService
    {
        private readonly IConfiguration _configuration;
        private readonly IAcombaConnection _connection;
        private AcoSDK.Customer CustomerInt = new AcoSDK.Customer();
        private AcombaX Acomba = new AcoSDK.AcombaX();
        public CustomerService(IConfiguration configuration, IAcombaConnection connection)
        {
            _configuration = configuration;
            _connection = connection;
        }
        public async Task<string> AddCustomerWebhook(CustomerRequestDto customerRequest)
        {
            try
            {
                _connection.OpenConnection();
                int error;

                CustomerInt.BlankCard();
                CustomerInt.BlankKey();

                //set customer primary key
                CustomerInt.PKey_CuNumber = customerRequest.customer_last_name + " " + customerRequest.customer_first_name;

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
                    if (error == 0)
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
        public async Task<string> AddCustomer(CustomerRequest customerRequest)
        {
            try
            {
                _connection.OpenConnection();
                int error;
                
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
        #region Import customers to acomba
        public async Task<List<string>> ImportCustomers(List<CustomerDto> customers)
        {
            var results = new List<string>();
            try
            {
                int error;
                AcoSDK.Customer CustomerInt = new AcoSDK.Customer();
                AcoSDK.AcombaX Acomba = new AcoSDK.AcombaX();
                foreach (var c in customers)
                {
                   
                    CustomerInt.BlankCard();
                    CustomerInt.BlankKey();
                    //set customer primary key
                    CustomerInt.PKey_CuNumber = c.customer_contact_no.phone;

                    //reserve primary key to add
                    error = CustomerInt.ReserveCardNumber();

                    if (error == 0)
                    {
                        CustomerInt.CuNumber = CustomerInt.PKey_CuNumber;
                        CustomerInt.CuSortKey = c.customer_last_name; //customerRequest.customer_last_name.Substring(0, 1);
                        CustomerInt.CuName = c.customer_first_name + " " + c.customer_last_name;
                        //CustomerInt.CuAddress = "test"; //customerRequest.customer_address.street;
                        // CustomerInt.CuCity = "test"; //customerRequest.customer_address.city;
                        // CustomerInt.CuPhoneNumber[(PhoneType)1] = "0912345678"; //customerRequest.customer_contact_no.mobile;
                        CustomerInt.CuActive = 1;

                        error = CustomerInt.AddCard();
                        if (error == 0)
                        {

                            string res = "Customer Id :" + c.customer_contact_no.phone + " successfully added.";
                            results.Add(res);
                        }
                        else
                        {
                            error = CustomerInt.FreeCardNumber();
                           
                            string res = "Customer Id :" + c.customer_contact_no.phone + "insert failed. Error :" + Acomba.GetErrorMessage(error);
                            results.Add(res);

                        }

                    }
                    else
                    {
                        string res = "Customer Id :" + c.customer_contact_no.phone + "insert failed. Error :" + Acomba.GetErrorMessage(error);
                        results.Add(res);
                    }
                }

                return results;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        #endregion
        #region Import customers to zuper
        public async Task<object> ImportCustomersToZuper()
        {
            int count = 1000; // number of customers to import
            int cardpos = 1; //CardPos of the first customer file to consult
            int error;
            
            var customerList = new List<CreateCustomerDto>();
            try
            {
                _connection.OpenConnection();

                error = CustomerInt.GetCards(cardpos, count);
                if(error == 0 || CustomerInt.CursorUsed > 0)
                {
                    for(int i = 0; i < CustomerInt.CursorUsed; i++)
                    {
                        CustomerInt.Cursor = i;
                        if(CustomerInt.CuStatus == 0)
                        {
                            var customFields = new List<CustomField>();
                            customFields.Add(new CustomField() { label = "CustomerNumber", value = CustomerInt.CuNumber });
                            var _customer = new CreateCustomerDto()
                            {
                                customer_email = CustomerInt.CuEMail[EMailType.EMail_One],
                                customer_first_name = CustomerInt.CuName,
                                customer_last_name = CustomerInt.CuSortKey,
                                customer_company_name = CustomerInt.CuName,
                                customer_contact_no = new CustomerContactNo()
                                {
                                    phone = CustomerInt.CuPhoneNumber[PhoneType.Ph_Number],
                                    work = CustomerInt.CuPhoneNumber[PhoneType.Ph_User1],
                                    mobile = CustomerInt.CuPhoneNumber[PhoneType.Ph_User2]
                                },
                                customer_address = new CustomerAddress()
                                {
                                    street = CustomerInt.CuAddress,
                                    city = CustomerInt.CuCity,
                                    state = CustomerInt.CuISOCountryCode,
                                    zip_code = CustomerInt.CuPostalCode
                                },
                                customer_billing_address = new CustomerBillingAddress()
                                {
                                    street = CustomerInt.CuAddress,
                                    city = CustomerInt.CuCity,
                                    state = CustomerInt.CuISOCountryCode,
                                    country = CustomerInt.CuISOCountryCode,
                                    zip_code = CustomerInt.CuPostalCode,

                                },
                                //custom_fields = customFields,
                                customer_category = "7a162a70-b2d3-11ed-b877-4f8e5e43701d"

                            };

                            customerList.Add(_customer);
                        }
                    }
                   
                }
                _connection.CloseConnection();
                var result = await ImportToZuper(customerList);
                return result;
            }
            catch (Exception ex)
            {
                _connection.CloseConnection();
                return ex.Message;
            }
           
           
        }
        private async Task<List<ResponseResult>> ImportToZuper(List<CreateCustomerDto> _customers)
        {
            var results = new List<ResponseResult>();

            foreach (var e in _customers)
            {
                var _reqBody = new Root()
                {
                    customer = e
                };
                var _http = new HttpClient();
                _http.DefaultRequestHeaders.Add("Accept", "application/json");
                _http.DefaultRequestHeaders.Add("x-api-key", _configuration["MetricApiKey"]);
                var request = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{_configuration["ZuperUrl"]}/customers"),
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(_reqBody), Encoding.UTF8, "application/json")
                };
                var response = await _http.SendAsync(request);
                //var response = await _http.PostAsJsonAsync($"{_configuration["ZuperUrl"]}/customers", _reqBody);
                var responseBody = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ResponseResult>(responseBody);
                results.Add(result);
            }

            return results;
        }
        #endregion
    }
}
