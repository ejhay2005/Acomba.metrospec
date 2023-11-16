namespace acomba.zuper_api.Dto
{
    public class TerritoryResponse : ResponseData
    {
        public List<TerritoryDto> data { get; set; }
    }
    public class TerritoryDto
    {
        public string territory_uid { get; set; }
        public string territory_name { get; set; }
        public string territory_description { get; set; }
        public string territory_color { get; set; }
        public string territory_type { get; set; }
        public TerritoryRadius territory_radius { get; set; }
        public List<string> territory_zipcodes { get; set; }
        public List<object> territory_coordinates { get; set; }
        public CreatedBy created_by { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string id { get; set; }
    }

    public class TerritoryRadius
    {
        public int radius { get; set; }
    }
}
