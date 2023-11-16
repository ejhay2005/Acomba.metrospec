namespace acomba.zuper_api.Dto
{
    public class QuotesResponse : ResponseData
    {
        public List<QuotesDto> data { get; set; }
    }
    public class Deposit
    {
    }
    public class QuotesDto
    {
        public string estimate_uid { get; set; }
        public DateTime estimate_date { get; set; }
        public DateTime expiry_date { get; set; }
        public Customer customer { get; set; }
        public Organization organization { get; set; }
        public object job { get; set; }
        public int total { get; set; }
        public int total_discount { get; set; }
        public List<object> tags { get; set; }
        public List<object> tax { get; set; }
        public string estimate_status { get; set; }
        public List<CustomField> custom_fields { get; set; }
        public Deposit deposit { get; set; }
        public bool is_expired { get; set; }
        public bool is_converted { get; set; }
        public bool is_deleted { get; set; }
        public bool is_active { get; set; }
        public CreatedBy created_by { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int estimate_no { get; set; }
        public string id { get; set; }
    }
}
