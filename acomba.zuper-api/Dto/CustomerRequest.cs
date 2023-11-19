using Newtonsoft.Json;

namespace acomba.zuper_api.Dto
{
    public class CustomerRequest
    {
       
        public string? customer { get; set; }


        public string? customer_first_name { get; set; }
       
        public string? customer_last_name { get; set; }
      
        public string? customer_email { get; set; }
       
        public string? company_uid { get; set; }
        public CustomerAddress? customer_address { get; set; }
        public CustomerContactNo? customer_contact_no { get; set; }
    }
}
