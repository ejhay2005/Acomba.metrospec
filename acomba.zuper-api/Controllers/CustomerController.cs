﻿using acomba.zuper_api.Authentication;
using acomba.zuper_api.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace acomba.zuper_api.Controllers
{
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration configuration;
        static string ZuperUrl;
        static List<CustomerDto> _customerList = new List<CustomerDto>();
        public CustomerController(IConfiguration configuration)
        {
            this.configuration = configuration;
            if (string.IsNullOrEmpty(ZuperUrl)) ZuperUrl = configuration["ZuperUrl"];
            
        }
        [HttpGet("get-customer")]
        public async Task<IActionResult> GetCustomer()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Accept", "application/json");
                    http.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}customers");
                    HttpResponseMessage response = await http.SendAsync(request);
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<CustomerResponse>(responseBody);

                    return Ok(result);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("add-customer")]
        public async Task<IActionResult> AddCustomer(CustomerDto customer)
        {
            try
            {
                _customerList.Add(customer);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        [HttpGet("get-added-customer")]
        public async Task<IActionResult> GetAddedCustomer()
        {
            return Ok(_customerList);
        }
    }
}