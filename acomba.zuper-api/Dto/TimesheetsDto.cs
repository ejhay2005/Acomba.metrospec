namespace acomba.zuper_api.Dto
{
    public class CreatedUser
    {
        public string user_uid { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public object external_login_id { get; set; }
        public object home_phone_number { get; set; }
        public string designation { get; set; }
        public string emp_code { get; set; }
        public object prefix { get; set; }
        public string work_phone_number { get; set; }
        public object mobile_phone_number { get; set; }
        public string profile_picture { get; set; }
        public object hourly_labor_charge { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public Role role { get; set; }

    }
    public class TimesheetsResponse : ResponseData
    {
        public string status { get; set; }
        public TimesheetsData data { get; set; }
    }
    public class TimesheetsData { 
        public int total_pages { get; set; }
        public int current_page { get; set; }
        public List<TimesheetsDto> timesheets { get; set; }
        public int total_records { get; set; }
    }

    public class TimesheetsDto
    {
        public string employee_timesheet_uid { get; set; }
        public string type_of_check { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public object auth_pic { get; set; }
        public object remarks { get; set; }
        public DateTime checked_time { get; set; }
        public DateTime created_at { get; set; }
        public Users user { get; set; }
        public CreatedUser created_user { get; set; }
        public CreatedUser triggered_by { get; set; }
        public object timesheet_location { get; set; }
        public string company_uid { get; set; }
    }

    public class Users
    {
        public string user_uid { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public object external_login_id { get; set; }
        public object home_phone_number { get; set; }
        public string designation { get; set; }
        public string emp_code { get; set; }
        public object prefix { get; set; }
        public string work_phone_number { get; set; }
        public object mobile_phone_number { get; set; }
        public string profile_picture { get; set; }
        public object hourly_labor_charge { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
}
