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
        static string ZuperUrl;
        static List<InvoiceRequest> _invoiceList = new List<InvoiceRequest>();
        public InvoiceController(IConfiguration configuration)
        {
            this.configuration = configuration;
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
        [HttpPost("add-invoice")]
        public async Task<IActionResult> AddInvoice(InvoiceRequest invoiceRequest)
        {
            try
            {
                _invoiceList.Add(invoiceRequest);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }
        [HttpGet("get-added-invoice")]
        public async Task<IActionResult> GetAddedInvoice()
        {
            
            return Ok(_invoiceList);
        }
    }
}
