using acomba.zuper_api.AcombaServices;
using acomba.zuper_api.Authentication;
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
        private readonly ICustomerService _customerService;
        static string ZuperUrl;
        static List<CustomerDto> _customerList = new List<CustomerDto>();
        public CustomerController(IConfiguration configuration,ICustomerService customerService)
        {
            this.configuration = configuration;
            if (string.IsNullOrEmpty(ZuperUrl)) ZuperUrl = configuration["ZuperUrl"];
            _customerService = customerService;
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
        public async Task<IActionResult> AddCustomer(CustomerRequestDto _cus)
        {
            try
            {
                //get customer details before saving to acomba
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Accept", "application/json");
                    http.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}customers/{_cus.customer}");
                    HttpResponseMessage response = await http.SendAsync(request);
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<CustomerDetailResponse>(responseBody);

                    var _addCustomer = await _customerService.AddCustomerWebhook(result.Data);
                    return Ok(_addCustomer);
                }

               
            }
            catch(Exception ex)
            {
                return BadRequest(ex.InnerException.Message == null ? ex.Message : ex.InnerException.Message);
            }
            
        }
        [HttpPut("update-customer")]
        public async Task<IActionResult> UpdateCustomer(CustomerRequest _cus)
        {
            try
            {

               
                //get customer details before updating to acomba
                using (var http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("Accept", "application/json");
                    http.DefaultRequestHeaders.Add("x-api-key", configuration["MetricApiKey"]);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{ZuperUrl}customers/{_cus.customer}");
                    HttpResponseMessage response = await http.SendAsync(request);
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<CustomerDetailResponse>(responseBody);

                    var updateCustomer = await _customerService.UpdateCustomer(result.Data);
                    return Ok(updateCustomer);
                }

            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message); 
            }
            
        }
        [HttpGet("import-customers-acomba")]
        public async Task<IActionResult> ImportCustomers()
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

                    var _import = _customerService.ImportCustomers(result.data);
                   
                    return Ok(_import);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("import-customers-zuper")]
        public async Task<IActionResult> ImportCustomersToZuper()
        {
            var result = await _customerService.ImportCustomersToZuper();
            return Ok(result);
        }
        
        [HttpGet("test-sdk")]
        public async Task<IActionResult> TestSdk()
        {
            AcoSDK.AcoSDKX AcoSDKInt = new AcoSDK.AcoSDKX();
            AcoSDK.AcombaX Acomba = new AcoSDK.AcombaX();
            AcoSDK.User UserInt = new AcoSDK.User();

            int Version;
            string CompanyPath;
            string AcombaPath;
            string MotDePasse;
            int Exist, Error;

            // Obtenir la version la plus récente du SDK
            Version = AcoSDKInt.VaVersionSDK;

            // Démarrer le SDK avec la version obtenue
            Error = AcoSDKInt.Start(Version);

            // Si le SDK est bien démarré
            if (Error == 0)
            {
                // Chemin d'accès de la société à ouvrir
                CompanyPath = configuration["CompanyPath"]; //"C:\\F1000.dta\\DemoSDK_EN";

                // Chemin d'accès des cartes d'enregistrement d'Acomba
                AcombaPath = configuration["AcombaPath"]; //"C:\\Aco_SDK";

                // Mot de passe de l'usager
                MotDePasse = configuration["Password"];//"DEMO";

                // Vérification de l'existence de la société à ouvrir
                Exist = Acomba.CompanyExists(CompanyPath);

                if (Exist != 0)
                {
                    // Ouverture de la société Demo
                    Error = Acomba.OpenCompany(AcombaPath, CompanyPath);

                    if (Error == 0)
                    {
                        // Recherche de l'usager "supervisor" pour trouver son CardPos
                        UserInt.PKey_UsNumber = configuration["Pkey"];
                        Error = UserInt.FindKey(1, false);

                        if (Error == 0)
                        {
                            // Connexion de l'usager "supervisor" avec son mot de passe

                            Error = Acomba.LogCurrentUser(UserInt.Key_UsCardPos, MotDePasse);

                            if (Error == 0)
                            {
                               
                                Console.WriteLine("Connexion de l'usager complétée avec succès.");
                                return Ok("Connexion de l'usager complétée avec succès.");
                            }
                            else
                            {
                                return BadRequest("Erreur: " + Acomba.GetErrorMessage(Error));
                                Console.WriteLine("Erreur: " + Acomba.GetErrorMessage(Error));
                            }
                        }
                        else
                        {
                            return BadRequest("Erreur: " + Acomba.GetErrorMessage(Error));
                            Console.WriteLine("Erreur: " + Acomba.GetErrorMessage(Error));
                        }
                    }
                    else
                    {
                        return BadRequest("Erreur: " + Acomba.GetErrorMessage(Error));
                        Console.WriteLine("Erreur: " + Acomba.GetErrorMessage(Error));
                    }
                }
                else
                {
                    return BadRequest("Dossier de la société invalide");
                    Console.WriteLine("Dossier de la société invalide");
                }
            }
            else
            {
                return BadRequest("Dossier de la société invalide");
                Console.WriteLine("Erreur: " + Acomba.GetErrorMessage(Error));
            }

        }
    }
}
