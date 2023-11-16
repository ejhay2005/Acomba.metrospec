namespace acomba.zuper_api.Dto
{
    public class TeamResponse : ResponseData
    {
        public List<TeamDto> data { get; set; }
        public class TeamDto
        {
            public string team_uid { get; set; }
            public string team_name { get; set; }
            public string team_color { get; set; }
            public string team_description { get; set; }
            public int user_count { get; set; }
            public int is_active { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }
    }
}
