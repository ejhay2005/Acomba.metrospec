using acomba.zuper_api.AcombaServices;
using acomba.zuper_api.Authentication;
using acomba.zuper_api.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace acomba.zuper_api.Controllers
{
    //[ServiceFilter(typeof(ApiKeyAuthFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IEmployeeService _employeeService;
        static string ZuperUrl;

        public EmployeeController(IEmployeeService employeeService, IConfiguration configuration)
        {
            _configuration = configuration;
            _employeeService = employeeService;
            if (string.IsNullOrEmpty(ZuperUrl)) ZuperUrl = _configuration["ZuperUrl"];
        }
        [HttpPost("add-employee")]
        public async Task<ActionResult> EmployeeAdd(EmployeeDto employee)
        {
            var _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("Accept", "application/json");
            _http.DefaultRequestHeaders.Add("x-api-key", _configuration["MetricApiKey"]);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}/user/{employee.user_uid}");
            HttpResponseMessage response = await _http.SendAsync(request);
            var responseBody = response.Content.ReadAsStringAsync().Result;
            var employeeDetails = JsonConvert.DeserializeObject<EmployeeResponse>(responseBody);

            employee.custom_fields = employeeDetails.data.custom_fields;

            var result = await _employeeService.AddEmployee(employee);
            return Ok(result);

        }
        [HttpPost("update-employee")]
        public async Task<ActionResult> EmployeeUpdate(EmployeeDto employee)
        {
            var result = await _employeeService.UpdateEmployee(employee);
            return Ok(result);
        }
        [HttpGet("import-employees-zuper")]
        public async Task<ActionResult> ImportEmployeesToZuper()
        {
            var result = await _employeeService.ExportEmployees();
            return Ok(result);
        }
    }
}
