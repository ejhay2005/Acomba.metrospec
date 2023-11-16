namespace acomba.zuper_api.Dto
{
    public class OrganizationResponse : ResponseData
    {
        public List<OrganizationDto> data { get; set; }
    }
    public class OrganizationDto
    {
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public string organization_name { get; set; }
        public string organization_description { get; set; }
        public string organization_logo { get; set; }
        public OrganizationAddress organization_address { get; set; }
        public string organization_uid { get; set; }
        public CreatedBy created_by { get; set; }
        public int no_of_customers { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }
   
 
}
