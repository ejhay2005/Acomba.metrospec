namespace acomba.zuper_api.Authentication
{
    public class ApiKeyAuthentication
    {
        private readonly RequestDelegate requestDelegate;
        private readonly IConfiguration configuration;
        public ApiKeyAuthentication(RequestDelegate requestDelegate,IConfiguration configuration)
        {
            this.requestDelegate = requestDelegate;
            this.configuration = configuration;
        }
        
        public async Task InvokeAsync(HttpContext httpContext)
        {
            
        }
    }
}
