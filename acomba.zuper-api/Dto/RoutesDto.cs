namespace acomba.zuper_api.Dto
{
    public class RoutesResponse : ResponseData
    {
        public List<RoutesDto> data { get; set; }
    }
    public class RoutesDto
    {
        public int total_jobs { get; set; }
        public string transport_mode { get; set; }
        public bool enable_traffic { get; set; }
        public bool is_optimized { get; set; }
        public bool is_locked { get; set; }
        public string company_id { get; set; }
        public CreatedBy created_by { get; set; }
        public string route_name { get; set; }
        public string route_uid { get; set; }
        public int duration { get; set; }
        public DateTime departure { get; set; }
        public DateTime route_end_time { get; set; }
        public string route_type { get; set; }
        public string color { get; set; }
        public StartLocation start_location { get; set; }
        public List<object> assigned_to { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class StartLocation
    {
        public string street { get; set; }
        public string name { get; set; }
        public List<double> geo_cords { get; set; }
    }
}
