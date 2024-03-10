using acomba.zuper_api.AcombaServices;
using acomba.zuper_api.Authentication;
using acomba.zuper_api.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        [HttpGet("get-invoices")]
        public async Task<IActionResult> GetInvoices()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Accept", "application/json");
                    http.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}invoice");
                    HttpResponseMessage response = await http.SendAsync(request);
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<InvoiceResponse>(responseBody);

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
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
            try
            {
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Accept", "application/json");
                    http.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}customers/{invoiceRequest.invoice.customer}");
                    HttpResponseMessage response = await http.SendAsync(request);
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    var _customer = JsonConvert.DeserializeObject<CustomerDetailResponse>(responseBody);

                    var result = await _invoiceService.AddInvoiceWebhook(invoiceRequest,_customer.Data);
                    return Ok(result);
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

               
                HttpRequestMessage customerRequest = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}customers/{invoiceData.data.customer.customer_uid}");
                HttpResponseMessage customerResponse = await http.SendAsync(customerRequest);
                var responseCustomerBody = customerResponse.Content.ReadAsStringAsync().Result;
                customerData = JsonConvert.DeserializeObject<CustomerDetailResponse>(responseCustomerBody);

                var result = await _invoiceService.CustomerPayment(_payment, customerData.Data);
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
    }
}
