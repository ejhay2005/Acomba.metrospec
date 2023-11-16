namespace acomba.zuper_api.Dto
{
    public class ServiceContractResponse : ResponseData
    {
        public List<ServiceContractDto> data { get; set;}
    }
    public class AwaitApprovalBy
    {
    }

    public class BillingPeriod
    {
        public string billing_period_uid { get; set; }
        public int billing_period_value { get; set; }
        public string billing_period_type { get; set; }
        public string billing_period_name { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
    }

    public class InvoiceSettings
    {
        public bool auto_generate { get; set; }
        public bool send_to_customer { get; set; }
        public BillingPeriod billing_period { get; set; }
        public PaymentTerm payment_term { get; set; }
        public string invoice_template { get; set; }
    }

    public class Organization
    {
        public string organization_uid { get; set; }
        public string organization_name { get; set; }
        public object organization_logo { get; set; }
        public object organization_description { get; set; }
        public object organization_email { get; set; }
        public int no_of_customers { get; set; }
        public OrganizationAddress organization_address { get; set; }
        public bool is_deleted { get; set; }
    }

    

    public class PaymentTerm
    {
        public string payment_term_uid { get; set; }
        public string payment_term_name { get; set; }
        public int no_of_days { get; set; }
    }

    public class ServiceContractDto
    {
        public string contract_uid { get; set; }
        public CustomerDto customer { get; set; }
        public Organization organization { get; set; }
        public string ref_no { get; set; }
        public string contract_name { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public int term_months { get; set; }
        public DateTime? activation_date { get; set; }
        public string approval_status { get; set; }
        public AwaitApprovalBy await_approval_by { get; set; }
        public List<CustomField> custom_fields { get; set; }
        public InvoiceSettings invoice_settings { get; set; }
        public double contract_subtotal { get; set; }
        public double contract_total { get; set; }
        public CreatedBy created_by { get; set; }
        public bool is_active { get; set; }
        public bool is_expired { get; set; }
        public bool is_deleted { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int contract_number { get; set; }
        public string id { get; set; }
    }


}
