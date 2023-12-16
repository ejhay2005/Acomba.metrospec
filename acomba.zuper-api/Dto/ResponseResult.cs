using static System.Runtime.InteropServices.JavaScript.JSType;

namespace acomba.zuper_api.Dto
{
    public class ResponseResult
    {
        public string type { get; set; }
        public string title { get; set; }
        public string message { get; set; }
        public object data { get; set; }
    }
}
