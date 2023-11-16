namespace acomba.zuper_api.Dto
{
    public class CustomField
    {
        public string label { get; set; }
        public string value { get; set; }
        public string type { get; set; }
        public bool hide_to_fe { get; set; }
        public bool hide_field { get; set; }
        public bool read_only { get; set; }
        public string group_name { get; set; }
        public string group_uid { get; set; }
        public string _id { get; set; }
    }
}
