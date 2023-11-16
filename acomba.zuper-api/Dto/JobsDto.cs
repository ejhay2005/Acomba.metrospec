namespace acomba.zuper_api.Dto
{
    public class JobsResponse : ResponseData
    {
        public List<JobsDto> data { get; set; }
    }
    public class AssignedTo
    {
        public User user { get; set; }
        public Team team { get; set; }
    }

    public class CurrentJobStatus
    {
        public string status_uid { get; set; }
        public string status_name { get; set; }
        public string status_type { get; set; }
        public string status_color { get; set; }
    }


    public class EstimatedDuration
    {
        public int hours { get; set; }
        public int minutes { get; set; }
    }

    public class JobCategory
    {
        public string category_name { get; set; }
        public string category_uid { get; set; }
        public string category_color { get; set; }
        public EstimatedDuration estimated_duration { get; set; }
    }

    public class JobStatus
    {
        public string status_uid { get; set; }
        public string status_name { get; set; }
        public string status_type { get; set; }
        public string status_color { get; set; }
        public List<object> checklist { get; set; }
        public DateTime created_at { get; set; }
    }


    public class Role
    {
        public int role_id { get; set; }
        public string role_uid { get; set; }
        public string role_name { get; set; }
        public string role_key { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class JobsDto
    {
        public string job_uid { get; set; }
        public Customer customer { get; set; }
        public string prefix { get; set; }
        public bool delayed_job { get; set; }
        public List<AssignedTo> assigned_to { get; set; }
        public string job_title { get; set; }
        public JobCategory job_category { get; set; }
        public string job_priority { get; set; }
        public string job_type { get; set; }
        public List<object> job_tags { get; set; }
        public DateTime scheduled_start_time { get; set; }
        public DateTime scheduled_end_time { get; set; }
        public CurrentJobStatus current_job_status { get; set; }
        public List<JobStatus> job_status { get; set; }
        public CustomerAddress customer_address { get; set; }
        public CustomerBillingAddress customer_billing_address { get; set; }
        public List<CustomField> custom_fields { get; set; }
        public List<object> products { get; set; }
        public bool is_recurrence { get; set; }
        public CreatedBy created_by { get; set; }
        public bool is_deleted { get; set; }
        public List<object> assigned_to_team { get; set; }
        public DateTime created_at { get; set; }
        public int work_order_number { get; set; }
        public object route { get; set; }
        public int scheduled_duration { get; set; }
    }

    public class Team
    {
        public string team_uid { get; set; }
        public string team_name { get; set; }
        public string team_color { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
    }

    public class User
    {
        public string user_uid { get; set; }
        public string emp_code { get; set; }
        public object prefix { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public string designation { get; set; }
        public object home_phone_number { get; set; }
        public object work_phone_number { get; set; }
        public object mobile_phone_number { get; set; }
        public string profile_picture { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public Role role { get; set; }
    }
    
}
