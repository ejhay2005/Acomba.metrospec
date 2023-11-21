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
        public string? customer_company_name { get; set; }
        public string? customer_description { get; set; }
        public string? customer_category { get; set; }
        public bool? visible_to_all { get; set; }
        public List<string>? customer_tags { get; set; }
        public bool? has_sla { get; set; }
        public SlaDurationDetails? sla_duration { get; set; }
        public CustomField? custom_fields { get; set; }
        public string? account_manager { get; set; }
        public CustomerAccountDetails? accounts { get; set; }
        public CustomerAddress? customer_address { get; set; }
        public CustomerBillingAddress? customer_billing_address { get; set; }
        public CustomerContactNo? customer_contact_no { get; set; }
    }
}
