namespace acomba.zuper_api.Dto
{
    public class InvoiceResponse : ResponseData
    {
        public List<InvoiceDto> data { get; set;}
    }
    
    public class Financing
    {
        public bool is_enabled { get; set; }
    }

    public class InvoiceDto
    {
        public string invoice_uid { get; set; }
        public DateTime invoice_date { get; set; }
        public DateTime due_date { get; set; }
        public Customer customer { get; set; }
        public object organization { get; set; }
        public string prefix { get; set; }
        public int total { get; set; }
        public List<object> tags { get; set; }
        public string invoice_status { get; set; }
        public List<object> custom_fields { get; set; }
        public ServiceContract service_contract { get; set; }
        public bool is_paid { get; set; }
        public bool is_deleted { get; set; }
        public bool is_active { get; set; }
        public CreatedBy created_by { get; set; }
        public Financing financing { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int invoice_no { get; set; }
        public string id { get; set; }
    }

    public class ServiceContract
    {
        public string contract_uid { get; set; }
        public string ref_no { get; set; }
        public string contract_name { get; set; }
        public string description { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public bool is_deleted { get; set; }
        public int contract_number { get; set; }
    }
}
