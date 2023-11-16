using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace acomba.zuper_api.Authentication
{
    public class ApiKeyAuthFilter : IAuthorizationFilter
    {
        private readonly IConfiguration _configuration;
        public ApiKeyAuthFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            
            if (!context.HttpContext.Request.Headers.TryGetValue(AuthenticationConst.ApiKeyHeaderName, out
                 var extractedApiKey))
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new UnauthorizedObjectResult("Unauthorize");
                return;
            }
            var apiKey = _configuration.GetValue<string>(AuthenticationConst.ApiSectionName);
            if (!apiKey.Equals(extractedApiKey))
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new UnauthorizedObjectResult("Invalid API Key");
                return;
            }
            
        }
    }
}
