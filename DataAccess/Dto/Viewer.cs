using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Dto
{
    public class Viewer
    {
        public int id { get; set; }
        public string country_code { get; set; }
        public string country_name { get; set; }
        public string city { get; set; }
        public string timezone { get; set; }
        public string device_type { get; set; }
        public string operating_system { get; set; }
        public string browser { get; set; }
        public string user_agent { get; set; }
        public string page_url { get; set; }
        public string referrer { get; set; }
        public DateTime visit_time { get; set; }
    }
}
