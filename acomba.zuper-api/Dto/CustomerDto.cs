using static System.Runtime.InteropServices.JavaScript.JSType;

namespace acomba.zuper_api.Dto
{
    public class CustomerResponse : ResponseData
    {
        public List<CustomerDto>? data { get; set; }
    }
    public class Accounts
    {
        public int? ltv { get; set; }
        public int? receivables { get; set; }
        public int? credits { get; set; }
    }

    public class CustomerAddress
    {
        public string? city { get; set; }
        public string? state { get; set; }
        public string? street { get; set; }
        public string? country { get; set; }
        public string? landmark { get; set; }
        public string? zip_code { get; set; }
        public List<double>? geo_cordinates { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? phone_number { get; set; }
        public string? email { get; set; }
        public string? _id { get; set; }
    }

    public class CustomerBillingAddress
    {
        public string? city { get; set; }
        public string? state { get; set; }
        public string? street { get; set; }
        public string? country { get; set; }
        public string? landmark { get; set; }
        public List<double>? geo_cordinates { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? phone_number { get; set; }
        public string? email { get; set; }
        public string? zip_code { get; set; }
        public string? _id { get; set; }
    }

    public class CustomerCategory
    {
        public string? category_uid { get; set; }
        public string? category_name { get; set; }
    }

    public class CustomerContactNo
    {
        public string? mobile { get; set; }
        public string? work { get; set; }
        public string? phone { get; set; }
        public string? _id { get; set; }
    }

    public class CustomerOrganization
    {
        public string? organization_uid { get; set; }
        public string? organization_name { get; set; }
        public object? organization_logo { get; set; }
        public object? organization_description { get; set; }
        public string? organization_email { get; set; }
        public OrganizationAddress organization_address { get; set; }
        public OrganizationBillingAddress organization_billing_address { get; set; }
        public bool? is_active { get; set; }
        public bool? is_deleted { get; set; }
    }

    public class OrganizationAddress
    {
        public string? city { get; set; }
        public string? state { get; set; }
        public string? street { get; set; }
        public string? country { get; set; }
        public string? landmark { get; set; }
        public List<double> geo_cordinates { get; set; }
        public string? zip_code { get; set; }
        public string? _id { get; set; }
    }

    public class OrganizationBillingAddress
    {
        public string? city { get; set; }
        public string? state { get; set; }
        public string? street { get; set; }
        public string? country { get; set; }
        public string? landmark { get; set; }
        public List<double> geo_cordinates { get; set; }
        public string? zip_code { get; set; }
        public string? _id { get; set; }
    }

    public class CustomerDto
    {
        public string? customer_uid { get; set; }
        public string? customer_first_name { get; set; }
        public string? customer_last_name { get; set; }
        public CustomerCategory? customer_category { get; set; }
        public string? customer_company_name { get; set; }
        public string? customer_email { get; set; }
        public int? no_of_jobs { get; set; }
        public List<string>? customer_tags { get; set; }
        public CustomerAddress? customer_address { get; set; }
        public CustomerBillingAddress? customer_billing_address { get; set; }
        public List<CustomField>? custom_fields { get; set; }
        public bool? has_sla { get; set; }
        public Accounts? accounts { get; set; }
        public bool? is_active { get; set; }
        public object? account_manager { get; set; }
        public bool? has_card_on_file { get; set; }
        public DateTime? created_at { get; set; }
        public CustomerContactNo? customer_contact_no { get; set; }
        public CustomerOrganization? customer_organization { get; set; }
        public string? id { get; set; }
    }
    public class CreateCustomerDto
    {
        public string? customer_uid { get; set; }
        public string? customer_first_name { get; set; }
        public string? customer_last_name { get; set; }
        public string customer_category { get; set; }
        public string? customer_company_name { get; set; }
        public string? customer_email { get; set; }
        //public int? no_of_jobs { get; set; }
        public List<string>? customer_tags { get; set; }
        public CustomerAddress? customer_address { get; set; }
        public CustomerBillingAddress? customer_billing_address { get; set; }
        public List<CustomField>? custom_fields { get; set; }
        public bool? has_sla { get; set; }
        public Accounts? accounts { get; set; }
        public bool? is_active { get; set; }
        public string? account_manager { get; set; }
        //public bool? has_card_on_file { get; set; }
        //public DateTime? created_at { get; set; }
        public CustomerContactNo? customer_contact_no { get; set; }
        public CustomerOrganization? customer_organization { get; set; }
        //public string? id { get; set; }
    }

    public class SlaDurationDetails
    {
        public int? days { get; set; }
        public int? hours { get; set; }
        public int? minutes { get; set; }
    }
    public class CustomerAccountDetails
    {
        public string? billing_frequency { get; set; }
        public string? payment_term { get; set; }
        public string? tax_group { get; set; }
    }
    public class Root
    {
        public CreateCustomerDto customer { get; set; }
    }
}
