namespace acomba.zuper_api.Dto
{
    public class EmployeeResponse : ResponseData
    {
        public EmployeeDto data { get; set; }
    }
    public class EmployeeDto
    {
        public string user_uid { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string? email { get; set; }
        public string? work_phone_number { get; set; }
        public string? home_phone_number { get; set; }
        public string? mobile_phone_number { get; set; }
        public string? emp_code { get; set; }
        public Role? role { get; set; }
        public string? external_id { get; set; }
        public string? profile_picture { get; set; }
        public bool is_active { get; set; }
        public string hourly_labor_charge { get; set; }
        public string? designation { get; set; }
        public string? prefix { get; set; }
        public string? company_uid { get; set; }
        public List<CustomField>? custom_fields { get; set; }
        public int role_id { get; set; }
        public string? password { get; set; }
        public string? confirm_password { get; set; }
        public List<WorkHour> work_hours { get; set; }
    }
    public class WorkHour
    {
        public string day { get; set; }
        public bool is_enabled { get; set; }
        public string start_time { get; set; }
        public string end_time { get; set; }
        public int work_mins { get; set; }
    }
}
