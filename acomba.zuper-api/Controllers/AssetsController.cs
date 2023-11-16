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
    public class AssetsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        static string ZuperUrl;
        public AssetsController(IConfiguration configuration)
        {
            this.configuration = configuration;
            if (string.IsNullOrEmpty(ZuperUrl)) ZuperUrl = configuration["ZuperUrl"];

        }
        [HttpGet("get-assets")]
        public async Task<IActionResult> GetAssets()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Accept", "application/json");
                    http.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}assets");
                    HttpResponseMessage response = await http.SendAsync(request);
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<AssetsReponse>(responseBody);

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
