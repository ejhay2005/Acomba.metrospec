namespace acomba.zuper_api.Dto
{
    public class CreatedBy
    {
        public string user_uid { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string home_phone_number { get; set; }
        public string designation { get; set; }
        public string emp_code { get; set; }
        public string work_phone_number { get; set; }
        public string profile_picture { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public Role role { get; set; }  
        public object external_login_id { get; set; }
        public object prefix { get; set; }
        public object hourly_labor_charge { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
