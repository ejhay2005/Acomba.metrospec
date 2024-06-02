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
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace acomba.zuper_api.AcombaServices
{
    public interface ICustomerService
    {
        Task<string> AddCustomerWebhook(CustomerDto customerRequest);
        Task<string> UpdateCustomer(CustomerDto _customer);
        Task<List<string>> ImportCustomers(List<CustomerDto> customers);
        Task<object> ImportCustomersToZuper();
        Task<object> GetAcombaCustomers(int cardpos, int niche, int activateNumcard);
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
                var _projectNumber = !_customer.custom_fields.Where(i => i.label.Contains("Project")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Project")).FirstOrDefault().value;
                var _customerType = !_customer.custom_fields.Where(i => i.label.Contains("Customer Type")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Customer Type")).FirstOrDefault().value;
                //var _paymentTerm = !_customer.custom_fields.Where(i => i.label.Contains("Payment Term")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Payment Term")).FirstOrDefault().value;
                var _receivable = !_customer.custom_fields.Where(i => i.label.Contains("Receivable")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Receivable")).FirstOrDefault().value;
                var _sortKey = !_customer.custom_fields.Where(i => i.label.Contains("Customer Sortkey")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Customer Sortkey")).FirstOrDefault().value;
                var _careOf = !_customer.custom_fields.Where(i => i.label.Contains("C/O")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("C/O")).FirstOrDefault().value;
                var _fax = !_customer.custom_fields.Where(i => i.label.Contains("Fax")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Fax")).FirstOrDefault().value;
                var _website = !_customer.custom_fields.Where(i => i.label.Contains("Website")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Website")).FirstOrDefault().value;
                var _personalEmail = !_customer.custom_fields.Where(i => i.label.Contains("Personal Email")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Personal Email")).FirstOrDefault().value;
                var _otherEmail = !_customer.custom_fields.Where(i => i.label.Contains("Other Email")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Other Email")).FirstOrDefault().value;
                var _taxGroup = !_customer.custom_fields.Where(i => i.label.Contains("Tax Group")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Tax Group")).FirstOrDefault().value;
                var _language = !_customer.custom_fields.Where(i => i.label.Contains("Language")).Any() ? null : _customer.custom_fields.Where(i => i.label.Contains("Language")).FirstOrDefault().value;
                var _priceId = !_customer.custom_fields.Where(i => i.label.Contains("Price Id")).Any() ? null : _customer.custom_fields.Where(i => i.label.Contains("Price Id")).FirstOrDefault().value; ;
                var _orderAllowed = !_customer.custom_fields.Where(i => i.label.Contains("Order Allowed")).Any() ? "No" : _customer.custom_fields.Where(i => i.label.Contains("Order Allowed")).FirstOrDefault().value;
                var _backOrder = !_customer.custom_fields.Where(i => i.label.Contains("Back Order Allowed")).Any() ? "No" : _customer.custom_fields.Where(i => i.label.Contains("Back Order Allowed")).FirstOrDefault().value;
                int? _statementId = !_customer.custom_fields.Where(i => i.label.Contains("Account Statement Id")).Any() || string.IsNullOrEmpty(_customer.custom_fields.Where(i => i.label.Contains("Account Statement Id")).FirstOrDefault().value) ? null : Convert.ToInt32(_customer.custom_fields.Where(i => i.label.Contains("Account Statement Id")).FirstOrDefault().value);
                var _interest = string.IsNullOrEmpty(_customer.custom_fields.Where(i => i.label.Contains("Interest")).FirstOrDefault().value) ? "0" : _customer.custom_fields.Where(i => i.label.Contains("Interest")).FirstOrDefault().value;

                if (_customerType == "Regular")
                {


                    var _matchTerm = string.IsNullOrEmpty(_customer.accounts.payment_term.payment_term_name) ? Regex.Match("1 - Upon Receipt", pattern) : Regex.Match(_customer.accounts.payment_term.payment_term_name, pattern);
                    var _matchReceivable = Regex.Match(_receivable, pattern);

                    CustomerInt.BlankCard();
                    CustomerInt.BlankKey();

                    //set customer primary key
                    CustomerInt.PKey_CuNumber = _customer.customer_contact_no.home;

                    //reserve primary key to add
                    error = CustomerInt.ReserveCardNumber();

                    if (error == 0)
                    {

                        CustomerInt.CuNumber = CustomerInt.PKey_CuNumber;
                        CustomerInt.CuSortKey = string.IsNullOrEmpty(_sortKey) ? _customer.customer_last_name : _sortKey; //customerRequest.customer_last_name.Substring(0, 1);
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
                        CustomerInt.CuProjectNumber = _projectNumber;
                        CustomerInt.CuCareOf = _careOf;
                        //CustomerInt.CuPhoneDescription[PhoneType.Ph_Fax] = "Fax";
                        //CustomerInt.CuPhoneNumber[PhoneType.Ph_Fax] = "514 376-0279" /*_fax*/;
                        CustomerInt.CuWebAddress = _website;
                        CustomerInt.CuEMail[EMailType.EMail_Two] = _personalEmail;
                        CustomerInt.CuEMail[EMailType.EMail_Three] = _otherEmail;
                        CustomerInt.CuTaxGroupNumber = _taxGroup;
                        CustomerInt.CuLanguage = GetLanguageId(_language);
                        CustomerInt.CuPriceLevel = PriceLevel.PL_Min;
                        CustomerInt.CuOrderAllowed = _orderAllowed == "No" ? 0 : 1;
                        CustomerInt.CuBoAllowed = _backOrder == "No" ? 0 : 1;
                        CustomerInt.CuStatement = StatementType.ST_Min;
                        CustomerInt.CuInterest = Convert.ToInt32(_interest) * 100;

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
                var _projectNumber = !_customer.custom_fields.Where(i => i.label.Contains("Project")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Project")).FirstOrDefault().value;
                var _customerType = !_customer.custom_fields.Where(i => i.label.Contains("Customer Type")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Customer Type")).FirstOrDefault().value;
                //var _paymentTerm = !_customer.custom_fields.Where(i => i.label.Contains("Payment Term")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Payment Term")).FirstOrDefault().value;
                var _receivable = !_customer.custom_fields.Where(i => i.label.Contains("Receivable")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Receivable")).FirstOrDefault().value;
                var _sortKey = !_customer.custom_fields.Where(i => i.label.Contains("Customer Sortkey")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Customer Sortkey")).FirstOrDefault().value;
                var _careOf = !_customer.custom_fields.Where(i => i.label.Contains("C/O")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("C/O")).FirstOrDefault().value;
                var _fax = !_customer.custom_fields.Where(i => i.label.Contains("Fax")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Fax")).FirstOrDefault().value;
                var _website = !_customer.custom_fields.Where(i => i.label.Contains("Website")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Website")).FirstOrDefault().value;
                var _personalEmail = !_customer.custom_fields.Where(i => i.label.Contains("Personal Email")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Personal Email")).FirstOrDefault().value;
                var _otherEmail = !_customer.custom_fields.Where(i => i.label.Contains("Other Email")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Other Email")).FirstOrDefault().value;
                var _taxGroup = !_customer.custom_fields.Where(i => i.label.Contains("Tax Group")).Any() ? string.Empty : _customer.custom_fields.Where(i => i.label.Contains("Tax Group")).FirstOrDefault().value;
                var _language = !_customer.custom_fields.Where(i => i.label.Contains("Language")).Any() ? null : _customer.custom_fields.Where(i => i.label.Contains("Language")).FirstOrDefault().value;
                var _priceId = !_customer.custom_fields.Where(i => i.label.Contains("Price Id")).Any() ? null : _customer.custom_fields.Where(i => i.label.Contains("Price Id")).FirstOrDefault().value; ;
                var _orderAllowed = !_customer.custom_fields.Where(i => i.label.Contains("Order Allowed")).Any() ? "No" : _customer.custom_fields.Where(i => i.label.Contains("Order Allowed")).FirstOrDefault().value;
                var _backOrder = !_customer.custom_fields.Where(i => i.label.Contains("Back Order Allowed")).Any() ? "No" : _customer.custom_fields.Where(i => i.label.Contains("Back Order Allowed")).FirstOrDefault().value;
                int? _statementId = !_customer.custom_fields.Where(i => i.label.Contains("Account Statement Id")).Any() || string.IsNullOrEmpty(_customer.custom_fields.Where(i => i.label.Contains("Account Statement Id")).FirstOrDefault().value) ? null : Convert.ToInt32(_customer.custom_fields.Where(i => i.label.Contains("Account Statement Id")).FirstOrDefault().value);
                var _interest = string.IsNullOrEmpty(_customer.custom_fields.Where(i => i.label.Contains("Interest")).FirstOrDefault().value) ? "0" : _customer.custom_fields.Where(i => i.label.Contains("Interest")).FirstOrDefault().value;

                if (_customerType == "Regular")
                {
                    CustomerInt.BlankKey();
                    CustomerInt.PKey_CuNumber = _customer.customer_contact_no.home;

                    error = CustomerInt.FindKey(noIndex, false);
                    if (error == 0)
                    {
                        var _matchTerm = string.IsNullOrEmpty(_customer.accounts.payment_term.payment_term_name) ? Regex.Match("1 - Upon Receipt", pattern) : Regex.Match(_customer.accounts.payment_term.payment_term_name, pattern);
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
                            CustomerInt.CuCareOf = _careOf;
                            //CustomerInt.CuPhoneNumber[PhoneType.Ph_Fax] = _fax;
                            CustomerInt.CuWebAddress = _website;
                            CustomerInt.CuEMail[EMailType.EMail_Two] = _personalEmail;
                            CustomerInt.CuEMail[EMailType.EMail_Three] = _otherEmail;
                            CustomerInt.CuTaxGroupNumber = _taxGroup;
                            CustomerInt.CuLanguage = GetLanguageId(_language);
                            CustomerInt.CuPriceLevel = PriceLevel.PL_Min;
                            CustomerInt.CuOrderAllowed = _orderAllowed == "No" ? 0 : 1;
                            CustomerInt.CuBoAllowed = _backOrder == "No" ? 0 : 1;
                            CustomerInt.CuStatement = StatementType.ST_Min;
                            CustomerInt.CuInterest = Convert.ToInt32(_interest) * 100;

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
                            error = CustomerInt.FreeCard();
                            return "Error: " + Acomba.GetErrorMessage(error);
                        }
                    }
                    else
                    {
                        error = CustomerInt.FreeCard();
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
                        var _matchTerm = string.IsNullOrEmpty(_customer.accounts.payment_term.payment_term_name) ? Regex.Match("1 - Upon Receipt", pattern) : Regex.Match(_customer.accounts.payment_term.payment_term_name, pattern);
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
                            error = CustomerInt.FreeCard();
                            return "Error: " + Acomba.GetErrorMessage(error);
                        }
                    }
                    else
                    {
                        error = CustomerInt.FreeCard();
                        return "Error :" + Acomba.GetErrorMessage(error);
                    }

                }
                else
                {
                    error = CustomerInt.FreeCard();
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
                if (error == 0 || CustomerInt.CursorUsed > 0)
                {
                    for (int i = 0; i < CustomerInt.CursorUsed; i++)
                    {
                        CustomerInt.Cursor = i;
                        if (CustomerInt.CuActive != 0)
                        {
                            var customFields = new List<CustomField>();
                            customFields.Add(new CustomField()
                            {
                                label = "Project",
                                value = GetProjectNumber(CustomerInt.CuProjectNumber),
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Customer Number",
                                value = CustomerInt.CuNumber,
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false,
                                group_name = "Additional Field",
                                group_uid = "994784d0-0966-11ef-9059-1f9ba808f712"
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Customer Sortkey",
                                value = CustomerInt.CuSortKey,
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false,
                                group_name = "Additional Field",
                                group_uid = "994784d0-0966-11ef-9059-1f9ba808f712"
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
                            customFields.Add(new CustomField()
                            {
                                label = "C/O",
                                value = CustomerInt.CuCareOf,
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false,
                                group_name = "Additional Field",
                                group_uid = "994784d0-0966-11ef-9059-1f9ba808f712"
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Fax",
                                value = CustomerInt.CuPhoneNumber[PhoneType.Ph_Fax],
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false,
                                group_name = "Additional Field",
                                group_uid = "994784d0-0966-11ef-9059-1f9ba808f712"
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Website",
                                value = CustomerInt.CuWebAddress,
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false,
                                group_name = "Additional Field",
                                group_uid = "994784d0-0966-11ef-9059-1f9ba808f712"
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Personal Email",
                                value = CustomerInt.CuEMail[EMailType.EMail_Two],
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false,
                                group_name = "Additional Field",
                                group_uid = "994784d0-0966-11ef-9059-1f9ba808f712"
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Other Email",
                                value = CustomerInt.CuEMail[EMailType.EMail_Three],
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false,
                                group_name = "Additional Field",
                                group_uid = "994784d0-0966-11ef-9059-1f9ba808f712"
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Tax Group",
                                value = CustomerInt.CuTaxGroupNumber,
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Language",
                                value = GetLanguage(CustomerInt.CuLanguage),
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false,
                                group_name = "Additional Field",
                                group_uid = "994784d0-0966-11ef-9059-1f9ba808f712"
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Price Id",
                                value = CustomerInt.CuPriceLevel.ToString(),
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false,
                                group_name = "Additional Field",
                                group_uid = "994784d0-0966-11ef-9059-1f9ba808f712"
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Order Allowed",
                                value = CustomerInt.CuOrderAllowed != 0 ? "Yes": "No",
                                type = "MULTI_ITEM",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false,
                                group_name = "Additional Field",
                                group_uid = "994784d0-0966-11ef-9059-1f9ba808f712"
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Back Order Allowed",
                                value = CustomerInt.CuBoAllowed != 0 ? "Yes" : "No",
                                type = "MULTI_ITEM",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false,
                                group_name = "Additional Field",
                                group_uid = "994784d0-0966-11ef-9059-1f9ba808f712"
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Account Statement Id",
                                value = CustomerInt.CuStatement.ToString(),
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false,
                                group_name = "Additional Field",
                                group_uid = "994784d0-0966-11ef-9059-1f9ba808f712"
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Interest",
                                value = (CustomerInt.CuInterest / 100).ToString(),
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false,
                                group_name = "Additional Field",
                                group_uid = "994784d0-0966-11ef-9059-1f9ba808f712"
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
                                is_active = Convert.ToBoolean(CustomerInt.CuActive),
                                has_sla = false,
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
        //get tax group
        private string TaxGroupCardpos(string tax)
        {
            AcoSDK.TaxGroup tg = new AcoSDK.TaxGroup();
            tg.PKey_TGNumber = tax;
            var error = tg.FindKey(1, false);
            if (error == 0)
            {
                var cardPos = tg.Key_TGCardPos;
                error = tg.GetCard(cardPos);
                if (error == 0)
                {
                    return tg.TGDescription;
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
        //get project number
        private string GetProjectNumber(string number)
        {
            AcoSDK.Project project = new AcoSDK.Project();
            project.PKey_PrNumber = number;

            var error = project.FindKey(1, false);
            if (error == 0)
            {
                var cardPos = project.Key_PrCardPos;
                error = project.GetCard(cardPos);
                if (error == 0)
                {
                    return project.PrNumber;
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
        //get Language
        private string GetLanguage(int language)
        {
            switch (language)
            {
                case 0:
                    return "Neutral";
                case 2:
                    return "Bulgarian";
                case 4:
                    return "Chinese";
                case 5:
                    return "Czech";
                case 6:
                    return "Danish";
                case 7:
                    return "German";
                case 8:
                    return "Greek";
                case 9:
                    return "English";
                case 10:
                    return "Spanish";
                case 11:
                    return "Finnish";
                case 12:
                    return "French";
                case 14:
                    return "Hungarian";
                case 15:
                    return "Icelandic";
                case 16:
                    return "Italian";
                case 17:
                    return "Japanese";
                case 18:
                    return "Korean";
                case 19:
                    return "Dutch";
                case 20:
                    return "Norwegian";
                case 21:
                    return "Polish";
                case 22:
                    return "Portuguese";
                case 24:
                    return "Romanian";
                case 25:
                    return "Russian";
                case 26:
                    return "Croatian";
                case 27:
                    return "Slovak";
                case 29:
                    return "Swedish";
                case 31:
                    return "Turkish";
                case 36:
                    return "Slovenian";
                default:
                    return "Neutral";
            }
        }
        private int GetLanguageId(string language)
        {
            switch (language)
            {
                case "Neutral":
                    return 0;
                case "Bulgarian":
                    return 2;
                case "Chinese":
                    return 4;
                case "Czech":
                    return 5;
                case "Danish":
                    return 6;
                case "German":
                    return 7;
                case "Greek":
                    return 8;
                case "English":
                    return 9;
                case "Spanish":
                    return 10;
                case "Finnish":
                    return 11;
                case "French":
                    return 12;
                case "Hungarian":
                    return 14;
                case "Icelandic":
                    return 15;
                case "Italian":
                    return 16;
                case "Japanese":
                    return 17;
                case "Korean":
                    return 18;
                case "Dutch":
                    return 19;
                case "Norwegian":
                    return 20;
                case "Polish":
                    return 21;
                case "Portuguese":
                    return 22;
                case "Romanian":
                    return 24;
                case "Russian":
                    return 25;
                case "Croatian":
                    return 26;
                case "Slovak":
                    return 27;
                case "Swedish":
                    return 29;
                case "Turkish":
                    return 31;
                case "Slovenian":
                    return 36;
                default:
                    return 0;
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
        #region get customers acomba
        public async Task<object> GetAcombaCustomers(int cardpos, int niche, int activateNumcard)
        {
            var result = new List<object>();
            
            try
            {
                _connection.OpenConnection();
                int total = 0; // number of customer to import
                // int cardpos = 8; //CardPos of the first product file to consult
                var totalCustomer = CustomerInt.NumCards();
                int error;
                if (activateNumcard == 1)
                {
                    total = totalCustomer;
                }
                else
                {
                    total = niche;
                }

                error = CustomerInt.GetCards(cardpos, total);

                if (error == 0 || CustomerInt.CursorUsed > 0)
                {
                    for (int i = 0; i < CustomerInt.CursorUsed; i++)
                    {
                        CustomerInt.Cursor = i;
                        if (CustomerInt.CuActive != 0)
                        {
                            var customFields = new List<CustomField>();
                            customFields.Add(new CustomField()
                            {
                                label = "Project",
                                value = GetProjectNumber(CustomerInt.CuProjectNumber),
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Customer Number",
                                value = CustomerInt.CuNumber,
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Customer Sortkey",
                                value = CustomerInt.CuSortKey,
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
                            customFields.Add(new CustomField()
                            {
                                label = "C/O",
                                value = CustomerInt.CuCareOf,
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Fax",
                                value = CustomerInt.CuPhoneNumber[PhoneType.Ph_Fax],
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Website",
                                value = CustomerInt.CuWebAddress,
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Personal Email",
                                value = CustomerInt.CuEMail[EMailType.EMail_Two],
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Other Email",
                                value = CustomerInt.CuEMail[EMailType.EMail_Three],
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Tax Group",
                                value = CustomerInt.CuTaxGroupNumber,
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Language Id",
                                value = CustomerInt.CuLanguage.ToString(),
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Price Id",
                                value = CustomerInt.CuPriceLevel.ToString(),
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Order Allowed",
                                value = CustomerInt.CuOrderAllowed.ToString(),
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Back Order Allowed",
                                value = CustomerInt.CuBoAllowed.ToString(),
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Account Statement Id",
                                value = CustomerInt.CuStatement.ToString(),
                                type = "SINGLE_LINE",
                                hide_field = false,
                                hide_to_fe = false,
                                read_only = false
                            });
                            customFields.Add(new CustomField()
                            {
                                label = "Interest",
                                value = (CustomerInt.CuInterest / 100).ToString() ,
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

                            result.Add(_customer);

                        }
                    }

                }
                _connection.CloseConnection();
                return result;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}