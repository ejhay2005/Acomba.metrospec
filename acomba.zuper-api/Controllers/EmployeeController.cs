using acomba.zuper_api.AcombaServices;
using acomba.zuper_api.Authentication;
using acomba.zuper_api.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace acomba.zuper_api.Controllers
{
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
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
            var result = await _employeeService.AddEmployee(employee);
            return Ok(result);

        }
        [HttpPost("update-employee")]
        public async Task<ActionResult> UpdateEmployee(EmployeeDto employee)
        {
            var result = await _employeeService.AddEmployee(employee);
            return Ok(result);
        }
        
    }
}
