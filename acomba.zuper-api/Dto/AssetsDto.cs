namespace acomba.zuper_api.Dto
{
    public class AssetsReponse : ResponseData
    {
        public List<AssetsDto> data { get; set; }
    }
    public class AssetCategory
    {
        public string category_uid { get; set; }
        public string category_name { get; set; }
        public bool is_deleted { get; set; }
    }

    public class AssetsDto
    {
        public string asset_uid { get; set; }
        public string asset_code { get; set; }
        public string asset_name { get; set; }
        public string asset_status { get; set; }
        public AssetCategory asset_category { get; set; }
        public string asset_serial_number { get; set; }
        public bool owned_by_customer { get; set; }
        public List<CustomField> custom_fields { get; set; }
        public bool is_deleted { get; set; }
        public bool is_active { get; set; }
        public CreatedBy created_by { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string id { get; set; }
    }
}
