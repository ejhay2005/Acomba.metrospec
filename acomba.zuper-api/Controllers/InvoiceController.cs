using acomba.zuper_api.AcombaServices;
using acomba.zuper_api.Authentication;
using acomba.zuper_api.Dto;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace acomba.zuper_api.Controllers
{
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IinvoiceService _invoiceService;
        static string ZuperUrl;
        static List<InvoiceRequest> _invoiceList = new List<InvoiceRequest>();
        public InvoiceController(IConfiguration configuration,IinvoiceService invoiceService)
        {
            this.configuration = configuration;
            _invoiceService = invoiceService;
            if (string.IsNullOrEmpty(ZuperUrl)) ZuperUrl = configuration["ZuperUrl"];

        }
        //[HttpGet("get-invoices")]
        //public async Task<IActionResult> GetInvoices()
        //{
        //    try
        //    {
        //        using (var http = new HttpClient())
        //        {
        //            http.DefaultRequestHeaders.Add("Accept", "application/json");
        //            http.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
        //            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}invoice");
        //            HttpResponseMessage response = await http.SendAsync(request);
        //            var responseBody = response.Content.ReadAsStringAsync().Result;
        //            var result = JsonConvert.DeserializeObject<InvoiceResponse>(responseBody);

        //            return Ok(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //[HttpPost("add-invoice-webhook")]
        //public async Task<IActionResult> AddInvoiceWebhook(InvoiceRequest invoiceRequest)
        //{
        //    try
        //    {
        //        _invoiceList.Add(invoiceRequest);
        //        var result = await _invoiceService.AddInvoiceWebhook(invoiceRequest);
        //        return Ok(result);
        //    }
        //    catch(Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
           
        //}
        [HttpPost("add-invoice")]
        public async Task<IActionResult> AddInvoice(InvoiceRequest invoiceRequest)
        {
            var result = new ResultDto();
            var invoiceResponse = new InvoiceResponse();
           
            try
            {
                using (var httpInvoice = new HttpClient())
                {
                    httpInvoice.DefaultRequestHeaders.Add("Accept", "application/json");
                    httpInvoice.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}invoice/{invoiceRequest.invoice_uid}");
                    HttpResponseMessage response = await httpInvoice.SendAsync(request);
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    invoiceResponse = JsonConvert.DeserializeObject<InvoiceResponse>(responseBody);

                }
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Accept", "application/json");
                    http.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}customers/{invoiceRequest.invoice.customer}");
                    HttpResponseMessage response = await http.SendAsync(request);
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    var _customer = JsonConvert.DeserializeObject<CustomerDetailResponse>(responseBody);

                     result = await _invoiceService.AddInvoiceWebhook(invoiceRequest,invoiceResponse, _customer.Data);
                }

                if (!string.IsNullOrEmpty(result.InvoiceID))
                {
                    //Get zuper invoice and update
                    //using (var http = new HttpClient())
                    //{
                    //    http.DefaultRequestHeaders.Add("Accept", "application/json");
                    //    http.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
                    //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}invoice/{invoiceRequest.invoice_uid}");
                    //    HttpResponseMessage response = await http.SendAsync(request);
                    //    var responseBody = response.Content.ReadAsStringAsync().Result;
                    //    invoiceResponse = JsonConvert.DeserializeObject<InvoiceResponse>(responseBody);

                    //}
                    var getCustomFields = invoiceResponse.data.custom_fields;
                    var updateInvoiceIdField = getCustomFields.Where(i => i.label == "Acomba Invoice ID").FirstOrDefault();
                    if (updateInvoiceIdField != null)
                    {
                        updateInvoiceIdField.value = result.InvoiceID;
                    }

                    using (var http = new HttpClient())
                    {
                        var _invoice = new Invoice()
                        {
                            customer = invoiceRequest.invoice.customer,
                            customer_billing_address = new CustomerBillingAddress
                            {
                                city = invoiceRequest.invoice.customer_billing_address.city,
                                street = invoiceRequest.invoice.customer_billing_address.street,
                                country = invoiceRequest.invoice.customer_billing_address.country,
                                zip_code = invoiceRequest.invoice.customer_billing_address.zip_code,
                                first_name = invoiceRequest.invoice.customer_billing_address.first_name,
                                last_name = invoiceRequest.invoice.customer_billing_address.last_name,
                                phone_number = invoiceRequest.invoice.customer_billing_address.phone_number,
                                email = invoiceRequest.invoice.customer_billing_address.email,
                                state = invoiceRequest.invoice.customer_billing_address.state
                            },
                            job = invoiceRequest.invoice.job,
                            prefix = invoiceRequest.invoice.prefix,
                            reference_no = invoiceRequest.invoice.reference_no,
                            invoice_date = invoiceRequest.invoice.invoice_date,
                            payment_term = invoiceRequest.invoice.payment_term,
                            remarks = invoiceRequest.invoice.remarks,
                            tags = invoiceRequest.invoice.tags,
                            template = invoiceRequest.invoice.template,
                            line_items = invoiceResponse.data.line_items,
                            sub_total = invoiceRequest.invoice.sub_total,
                            attachments = invoiceRequest.invoice.attachments,
                            notes = invoiceRequest.invoice.notes,
                            discount = invoiceRequest.invoice.discount,
                            tax = invoiceRequest.invoice.tax,
                            custom_fields = getCustomFields.ToList(),
                            due_date = invoiceRequest.invoice.due_date,


                        };

                        var putInvoiceRequest = new
                        {
                            invoice = _invoice
                        };
                        
                        string serialize = JsonConvert.SerializeObject(putInvoiceRequest);
                        http.DefaultRequestHeaders.Add("Accept", "application/json");
                        http.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
                        var content = new StringContent(serialize, Encoding.UTF8, "application/json");
                        HttpRequestMessage requestUpdate = new HttpRequestMessage(HttpMethod.Put, $"{ZuperUrl}invoice/{invoiceRequest.invoice_uid}");
                        requestUpdate.Content = content;
                        HttpResponseMessage updateInvoice = await http.SendAsync(requestUpdate);
                        //HttpResponseMessage updateInvoice = await http.PutAsJsonAsync($"{ZuperUrl}invoice/{invoiceRequest.invoice_uid}", _invoice);
                        var updateResponseBody = updateInvoice.Content.ReadAsStringAsync().Result;



                        return Ok(result);
                    }
                }
                else
                {
                    return BadRequest(result.Message);
                }
                 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpPost("invoice-payment")]
        public async Task<IActionResult> InvoicePayment(InvoicePayment _payment)
        {
            var customerData = new CustomerDetailResponse();
           
            using (var http = new HttpClient())
            {

                http.DefaultRequestHeaders.Add("Accept", "application/json");
                http.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}invoice/{_payment.invoice_uid}");
                HttpResponseMessage response = await http.SendAsync(request);
                dynamic invoiceData = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

                string paymentMode = invoiceData.data.payment_mode.payment_mode_name;


                HttpRequestMessage customerRequest = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}customers/{invoiceData.data.customer.customer_uid}");
                HttpResponseMessage customerResponse = await http.SendAsync(customerRequest);
                var responseCustomerBody = customerResponse.Content.ReadAsStringAsync().Result;
                customerData = JsonConvert.DeserializeObject<CustomerDetailResponse>(responseCustomerBody);

                var result = await _invoiceService.CustomerPayment(_payment, customerData.Data, paymentMode);
                return Ok(result);
            }

            
        }
        [HttpGet("get-acomba-invoice")]
        public async Task<IActionResult> GetAcombaInvoice(string invoiceId)
        {
            try
            {
                var _get = await _invoiceService.GetInvoice(invoiceId);

                return Ok(_get);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet("get-acomba-invoices")]
        public async Task<IActionResult> GetAcombaInvoices()
        {
            try
            {
                var _get = await _invoiceService.GetInvoices();

                return Ok(_get);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
        [HttpGet("get-zuper-invoice")]
        public async Task<IActionResult> GetZuperInvoice(string invoice_uid)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Accept", "application/json");
                    http.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}invoice/{invoice_uid}");
                    HttpResponseMessage response = await http.SendAsync(request);
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    var _invoiceResponse = JsonConvert.DeserializeObject<InvoiceResponse>(responseBody);

                    return Ok(_invoiceResponse.data);
                }

               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
