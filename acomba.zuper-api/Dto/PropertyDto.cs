namespace acomba.zuper_api.Dto
{
    public class PropertyResponse : ResponseData
    {
        public List<PropertyDto> data { get; set; }
    }
    public class Customer
    {
        public string customer_last_name { get; set; }
        public string customer_email { get; set; }
        public string customer_first_name { get; set; }
        public string customer_uid { get; set; }
        public CustomerContactNo customer_contact_no { get; set; }
    }

    public class PropertyAddress
    {
        public string street { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zip_code { get; set; }
        public string city { get; set; }
        public List<double> geo_cordinates { get; set; }
    }

    public class PropertyCustomer
    {
        public Customer customer { get; set; }
    }

    public class PropertyOrganization
    {
        public string organization_name { get; set; }
        public string organization_logo { get; set; }
        public string organization_uid { get; set; }
    }

    public class PropertyDto
    {
        public int no_of_jobs { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public string property_name { get; set; }
        public PropertyAddress property_address { get; set; }
        public List<PropertyCustomer> property_customers { get; set; }
        public List<CustomField> custom_fields { get; set; }
        public string property_image { get; set; }
        public PropertyOrganization property_organization { get; set; }
        public string property_uid { get; set; }
        public CreatedBy created_by { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

}
