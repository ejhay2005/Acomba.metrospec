using acomba.zuper_api.Authentication;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace acomba.zuper_api.Controllers
{
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationController : ControllerBase
    {
        [HttpGet("test-api")]
        public async Task<string> TestApi()
        {
            return "Authorize";
        }
        [HttpPost("change-setting")]
        public async Task<IActionResult> ChangeSetting()
        {
            return Ok("");
        }
    }
}
