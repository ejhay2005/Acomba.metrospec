using acomba.zuper_api.AcombaServices;
using acomba.zuper_api.Authentication;
using acomba.zuper_api.Dto;
using acomba.zuper_api.Models;
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
        private readonly DbService dbService;
        private readonly ICustomerService customerService;
        static string ZuperUrl;
        static List<CustomerDto> _customerList = new List<CustomerDto>();
        public CustomerController(IConfiguration configuration, DbService dbService,ICustomerService customerService)
        {
            this.configuration = configuration;
            if (string.IsNullOrEmpty(ZuperUrl)) ZuperUrl = configuration["ZuperUrl"];
            this.dbService = dbService;
            this.customerService = customerService;
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
        public async Task<IActionResult> AddCustomer(CustomerRequest _cus)
        {
            try
            {
                Models.Customer cus = new Models.Customer()
                {
                    CustomerId = _cus.customer,
                    CustomerFirstName = _cus.customer_first_name,
                    CustomerLastName = _cus.customer_last_name,
                    CustomerEmail = _cus.customer_email,
                    CompanyUid = _cus.company_uid
                };
                dbService.Customers.Add(cus);
                await dbService.SaveChangesAsync();

                //save customer data in acomba 
               var result = await customerService.AddCustomer(_cus);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        [HttpPut("update-customer")]
        public async Task<IActionResult> UpdateCustomer(CustomerRequest _cus)
        {
            try
            {
                var update = dbService.Customers.Where(i => i.CustomerId == _cus.customer).FirstOrDefault();
                if (update != null)
                {
                    update.CustomerEmail = _cus.customer_email;
                    update.CustomerFirstName = _cus.customer_first_name;
                    update.CustomerLastName = _cus.customer_last_name;
                    update.CompanyUid = _cus.company_uid;

                    dbService.Customers.Update(update);
                    await dbService.SaveChangesAsync();

                    //update customer in acomba
                    var result = await customerService.UpdateCustomer(_cus);
                    return Ok(result);
                }
                else
                {
                    return BadRequest("No Customer found!");
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message); 
            }
            
        }
        [HttpGet("get-added-customer")]
        public async Task<IActionResult> GetAddedCustomer()
        {
            var getCustomers = dbService.Customers.ToList();
            return Ok(getCustomers);
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
                        UserInt.PKey_UsNumber = "supervisor";
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
