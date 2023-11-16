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
    public class ServiceContractController : ControllerBase
    {
        private readonly IConfiguration configuration;
        static string ZuperUrl;
        public ServiceContractController(IConfiguration configuration)
        {
            this.configuration = configuration;
            if (string.IsNullOrEmpty(ZuperUrl)) ZuperUrl = configuration["ZuperUrl"];

        }
        [HttpGet("get-service-contracts")]
        public async Task<IActionResult> GetServiceContracts()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Accept", "application/json");
                    http.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}service_contract");
                    HttpResponseMessage response = await http.SendAsync(request);
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<ServiceContractResponse>(responseBody);

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
