using acomba.zuper_api.Dto;
using AcoSDK;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using System.Text;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;
using acomba.zuper_api.Helper;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace acomba.zuper_api.AcombaServices
{
    public interface ICustomerService
    {
        Task<string> AddCustomerWebhook(CustomerDto customerRequest);
        Task<string> UpdateCustomer(CustomerDto _customer);
        Task<List<string>> ImportCustomers(List<CustomerDto> customers);
        Task<object> ImportCustomersToZuper();
    }
    public class CustomerService : ICustomerService
    {
        private readonly IConfiguration _configuration;
        private readonly IAcombaConnection _connection;
        private readonly ICountryHelper _countryHelper;
        private AcoSDK.Customer CustomerInt = new AcoSDK.Customer();
        private AcombaX Acomba = new AcoSDK.AcombaX();
        public CustomerService(IConfiguration configuration, IAcombaConnection connection, ICountryHelper countryHelper)
        {
            _configuration = configuration;
            _connection = connection;
            _countryHelper = countryHelper;
        }
        public async Task<string> AddCustomerWebhook(CustomerDto _customer)
        {
           
            string pattern = @"\d+";
            try
            {
                _connection.OpenConnection();
                int error;

                //getting custom field data
                var _projectNumber = _customer.custom_fields.Where(i => i.label.Contains("Project")).FirstOrDefault().value;
                var _customerType = _customer.custom_fields.Where(i => i.label.Contains("Customer Type")).FirstOrDefault().value;
                var _paymentTerm = _customer.custom_fields.Where(i => i.label.Contains("Payment Term")).FirstOrDefault().value;
                var _receivable = _customer.custom_fields.Where(i => i.label.Contains("Receivable")).FirstOrDefault().value;

                if (_customerType == "Regular")
                {


                    var _matchTerm = Regex.Match(_paymentTerm, pattern);
                    var _matchReceivable = Regex.Match(_receivable, pattern);

                    CustomerInt.BlankCard();
                    CustomerInt.BlankKey();

                    //set customer primary key
                    CustomerInt.PKey_CuNumber = _projectNumber;

                    //reserve primary key to add
                    error = CustomerInt.ReserveCardNumber();

                    if (error == 0)
                    {

                        CustomerInt.CuNumber = CustomerInt.PKey_CuNumber;
                        CustomerInt.CuSortKey = _customer.customer_last_name; //customerRequest.customer_last_name.Substring(0, 1);
                        CustomerInt.CuName = _customer.customer_first_name + " " + _customer.customer_last_name;
                        CustomerInt.CuAddress = _customer.customer_address.street;
                        CustomerInt.CuCity = _customer.customer_address.city;
                        CustomerInt.CuPostalCode = _customer.customer_address.zip_code;
                        CustomerInt.CuEMail[EMailType.EMail_One] = _customer.customer_email;
                        CustomerInt.CuISOCountryCode = string.IsNullOrEmpty(_customer.customer_address.country)? string.Empty : _countryHelper.GetCountryCode(_customer.customer_address.country);
                        CustomerInt.CuPhoneNumber[PhoneType.Ph_Number] = _customer.customer_contact_no.home;
                        CustomerInt.CuActive = 1;
                        CustomerInt.CuTermNumber = Convert.ToInt32(_matchTerm.Value);
                        CustomerInt.CuReceivable = Convert.ToInt32(_matchReceivable.Value);
                        
                        error = CustomerInt.AddCard();
                        if (error == 0)
                        {
                            _connection.CloseConnection();
                            return "Addition completed successfully";
                        }
                        else
                        {
                            error = CustomerInt.FreeCardNumber();
                            _connection.CloseConnection();
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
                        error = CustomerInt.FreeCardNumber();
                        _connection.CloseConnection();
                        return "Error:" + Acomba.GetErrorMessage(error);
                    }
                }
                else if(_customerType == "Supplier")
                {
                    var SupplierInt = new AcoSDK.Supplier();

                    SupplierInt.BlankCard();
                    SupplierInt.BlankKey();

                    SupplierInt.PKey_SuNumber = _projectNumber;

                    error = SupplierInt.ReserveCardNumber();

                    if(error == 0)
                    {
                        SupplierInt.SuNumber = _projectNumber;
                        SupplierInt.SuSortKey = _customer.customer_last_name;
                        SupplierInt.SuName = _customer.customer_first_name + " " + _customer.customer_last_name;
                        SupplierInt.SuAddress = _customer.customer_address.street;
                        SupplierInt.SuCity = _customer.customer_address.city;
                        SupplierInt.SuPostalCode = _customer.customer_address.zip_code;
                        SupplierInt.SuEMail[EMailType.EMail_One] = _customer.customer_email;
                        SupplierInt.SuISOCountryCode = string.IsNullOrEmpty(_customer.customer_address.country) ? string.Empty : _countryHelper.GetCountryCode(_customer.customer_address.country);
                        SupplierInt.SuPhoneNumber[PhoneType.Ph_Number] = _customer.customer_contact_no.home;
                        SupplierInt.SuActive = 1;
                        //SupplierInt.SuPaymentTermNumber = Convert.ToInt32(_matchTerm.Value);
                        //SupplierInt.SuPaymentTermNumber = 

                        error = SupplierInt.AddCard();
                        if (error == 0)
                        {
                            _connection.CloseConnection();
                            return "Addition completed successfully";
                        }
                        else
                        {
                            error = SupplierInt.FreeCardNumber();
                            _connection.CloseConnection();
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
                        error = CustomerInt.FreeCardNumber();
                        _connection.CloseConnection();
                        return "Error:" + Acomba.GetErrorMessage(error);
                    }

                }
                else
                {
                    return "";
                }

            }
            catch (Exception ex)
            {
                var error = CustomerInt.FreeCardNumber();
                _connection.CloseConnection();
                return "Error:" + ex.Message  ;
            }
        }
        
        
        public async Task<string> UpdateCustomer(CustomerDto _customer)
        {
            try
            {
                _connection.OpenConnection();
                const int noIndex = 1;
                int error,cardpos;
                string pattern = @"\d+";

                //getting custom field data
                var _projectNumber = _customer.custom_fields.Where(i => i.label.Contains("Project")).FirstOrDefault().value;
                var _customerType = _customer.custom_fields.Where(i => i.label.Contains("Customer Type")).FirstOrDefault().value;
                var _paymentTerm = _customer.custom_fields.Where(i => i.label.Contains("Payment Term")).FirstOrDefault().value;
                var _receivable = _customer.custom_fields.Where(i => i.label.Contains("Receivable")).FirstOrDefault().value;

                if (_customerType == "Regular")
                {
                    CustomerInt.BlankKey();
                    CustomerInt.PKey_CuNumber = _projectNumber;

                    error = CustomerInt.FindKey(noIndex, false);
                    if (error == 0)
                    {
                        var _matchTerm = Regex.Match(_paymentTerm, pattern);
                        var _matchReceivable = Regex.Match(_receivable, pattern);

                        cardpos = CustomerInt.Key_CuCardPos;
                        error = CustomerInt.ReserveCard(cardpos);

                        if (error == 0)
                        {

                            CustomerInt.CuNumber = CustomerInt.PKey_CuNumber;
                            CustomerInt.CuSortKey = _customer.customer_last_name; //customerRequest.customer_last_name.Substring(0, 1);
                            CustomerInt.CuName = _customer.customer_first_name + " " + _customer.customer_last_name;
                            CustomerInt.CuAddress = _customer.customer_address.street;
                            CustomerInt.CuCity = _customer.customer_address.city;
                            CustomerInt.CuPostalCode = _customer.customer_address.zip_code;
                            CustomerInt.CuEMail[EMailType.EMail_One] = _customer.customer_email;
                            CustomerInt.CuISOCountryCode = string.IsNullOrEmpty(_customer.customer_address.country) ? string.Empty : _countryHelper.GetCountryCode(_customer.customer_address.country);
                            CustomerInt.CuPhoneNumber[PhoneType.Ph_Number] = _customer.customer_contact_no.home;
                            CustomerInt.CuActive = 1;
                            CustomerInt.CuTermNumber = Convert.ToInt32(_matchTerm.Value);
                            CustomerInt.CuReceivable = Convert.ToInt32(_matchReceivable.Value);

                            error = CustomerInt.ModifyCard(true);
                            if (error == 0)
                            {
                                return "Update completed successfully";
                            }
                            else
                            {
                                error = CustomerInt.FreeCard();
                                if (error != 0)
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
                else if(_customerType == "Supplier")
                {
                    var SupplierInt = new AcoSDK.Supplier();

                    SupplierInt.BlankKey();
                    SupplierInt.PKey_SuNumber = _projectNumber;

                    error = SupplierInt.FindKey(noIndex, false);
                    if(error == 0)
                    {
                        var _matchTerm = Regex.Match(_paymentTerm, pattern);
                        var _matchReceivable = Regex.Match(_receivable, pattern);

                        cardpos = SupplierInt.Key_SuCardPos;
                        error = SupplierInt.ReserveCard(cardpos);

                        if(error == 0)
                        {
                            SupplierInt.SuNumber = _projectNumber;
                            SupplierInt.SuSortKey = _customer.customer_last_name;
                            SupplierInt.SuName = _customer.customer_first_name + " " + _customer.customer_last_name;
                            SupplierInt.SuAddress = _customer.customer_address.street;
                            SupplierInt.SuCity = _customer.customer_address.city;
                            SupplierInt.SuPostalCode = _customer.customer_address.zip_code;
                            SupplierInt.SuEMail[EMailType.EMail_One] = _customer.customer_email;
                            SupplierInt.SuISOCountryCode = string.IsNullOrEmpty(_customer.customer_address.country) ? string.Empty : _countryHelper.GetCountryCode(_customer.customer_address.country);
                            SupplierInt.SuPhoneNumber[PhoneType.Ph_Number] = _customer.customer_contact_no.home;
                            SupplierInt.SuActive = 1;
                            //SupplierInt.SuPaymentTermNumber = Convert.ToInt32(_matchTerm.Value);
                            //SupplierInt.SuPaymentTermNumber = 

                            SupplierInt.ModifyCard(true);
                            if (error == 0)
                            {
                                return "Update completed successfully";
                            }
                            else
                            {
                                error = CustomerInt.FreeCard();
                                if (error != 0)
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
                else
                {
                    return "";
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
                    CustomerInt.PKey_CuNumber = c.customer_contact_no.home;

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

                            string res = "Customer Id :" + c.customer_contact_no.home + " successfully added.";
                            results.Add(res);
                        }
                        else
                        {
                            error = CustomerInt.FreeCardNumber();
                           
                            string res = "Customer Id :" + c.customer_contact_no.home + "insert failed. Error :" + Acomba.GetErrorMessage(error);
                            results.Add(res);

                        }

                    }
                    else
                    {
                        string res = "Customer Id :" + c.customer_contact_no.home + "insert failed. Error :" + Acomba.GetErrorMessage(error);
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
            int total = 0; // number of customers to import
            int cardpos = 2; //CardPos of the first customer file to consult
            int error;
           
            var customerList = new List<CreateCustomerDto>();
            try
            {
                _connection.OpenConnection();
                total = CustomerInt.NumCards();

                error = CustomerInt.GetCards(cardpos, total);
                if(error == 0 || CustomerInt.CursorUsed > 0)
                {
                    for(int i = 0; i < CustomerInt.CursorUsed; i++)
                    {
                        CustomerInt.Cursor = i;
                        if (CustomerInt.CuStatus == 0 )
                        {
                            var customFields = new List<CustomField>();
                            customFields.Add(new CustomField() 
                            { label = "Project",
                              value = CustomerInt.CuNumber,
                              type = "SINGLE_LINE",
                              hide_field = false,
                              hide_to_fe = false,
                              read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Customer Type",
                                value = "Regular",
                                type = "SINGLE_ITEM",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Payment Term",
                                value = CustomerInt.CuTermNumber + " - " + PaymentTermCardpos(CustomerInt.CuTermNumber),
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Receivable",
                                value = "0 - New TD 004-4440-5299025",
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            var _customer = new CreateCustomerDto()
                            {
                                customer_email = CustomerInt.CuEMail[EMailType.EMail_One],
                                customer_first_name = CustomerInt.CuName,
                                customer_last_name = CustomerInt.CuSortKey,
                                customer_company_name = CustomerInt.CuName,
                                customer_contact_no = new CustomerContactNo()
                                {
                                    home = CustomerInt.CuPhoneNumber[PhoneType.Ph_Number],
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
                                custom_fields = customFields,
                                customer_category = "7a162a70-b2d3-11ed-b877-4f8e5e43701d",
                                is_active = Convert.ToBoolean(CustomerInt.CuActive)
                                
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
        //getting payment term detail 
        private string PaymentTermCardpos(int term)
        {
            AcoSDK.CustomerTerm ct = new AcoSDK.CustomerTerm();
            ct.PKey_CTNumber = term;
            var error = ct.FindKey(1, false);
            if(error == 0)
            {
                Console.WriteLine(ct.Key_CTDescription);
                var cardPos = ct.Key_CTCardPos;
                error = ct.GetCard(cardPos);
                if(error == 0)
                {
                    return ct.CTDescription;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
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
