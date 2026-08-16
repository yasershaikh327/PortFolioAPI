using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace PortFolioAPI.Models
{
    public class ViewerDto
    {
        [Key]
        public int id { get; set; }

        [Required, StringLength(10)]
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "Invalid country code")]
        public string country_code { get; set; }

        [Required, StringLength(100)]
        public string country_name { get; set; }

        [Required, StringLength(100)]
        public string city { get; set; }

        [Required, StringLength(100)]
        public string timezone { get; set; }

        [Required, StringLength(50)]
        public string device_type { get; set; }

        [Required, StringLength(50)]
        public string operating_system { get; set; }

        [Required, StringLength(100)]
        public string browser { get; set; }

        [Required, StringLength(500)]
        public string user_agent { get; set; }

        [Required, StringLength(500)]
        [RegularExpression(@"^https?:\/\/[^\s]+$", ErrorMessage = "Invalid URL format")]
        public string page_url { get; set; }

        [StringLength(500)]
        public string referrer { get; set; }

        [Required]
        public DateTime visit_time { get; set; }

        // 🔒 Sanitize to protect against XSS
        public void Sanitize()
        {
            country_code = HtmlEncode(country_code);
            country_name = HtmlEncode(country_name);
            city = HtmlEncode(city);
            timezone = HtmlEncode(timezone);
            device_type = HtmlEncode(device_type);
            operating_system = HtmlEncode(operating_system);
            browser = HtmlEncode(browser);
            user_agent = HtmlEncode(user_agent);
            page_url = HtmlEncode(page_url);
            referrer = HtmlEncode(referrer);
        }

        private static string HtmlEncode(string input)
        {
            return string.IsNullOrEmpty(input) ? input : HttpUtility.HtmlEncode(input);
        }
    }



    public static class ViewerHelper
    {
        public static async Task<ViewerDto> GetUserDetailsAsync(string userAgent, string pageUrl, string referrer)
        {
            var details = new ViewerDto
            {
                user_agent = userAgent,
                page_url = pageUrl,
                referrer = string.IsNullOrEmpty(referrer) ? "Direct Visit" : referrer,
                timezone = System.TimeZoneInfo.Local.Id
            };

            // Device type
            if (Regex.IsMatch(userAgent, "tablet|ipad", RegexOptions.IgnoreCase))
                details.device_type = "Tablet";
            else if (Regex.IsMatch(userAgent, "mobile|android|iphone|ipod", RegexOptions.IgnoreCase))
                details.device_type = "Mobile";
            else
                details.device_type = "Desktop";

            // Operating system
            if (Regex.IsMatch(userAgent, "windows nt", RegexOptions.IgnoreCase))
                details.operating_system = "Windows";
            else if (Regex.IsMatch(userAgent, "macintosh|mac os x", RegexOptions.IgnoreCase) &&
                     !Regex.IsMatch(userAgent, "iphone|ipad|ipod", RegexOptions.IgnoreCase))
                details.operating_system = "macOS";
            else if (Regex.IsMatch(userAgent, "android", RegexOptions.IgnoreCase))
                details.operating_system = "Android";
            else if (Regex.IsMatch(userAgent, "iphone|ipad|ipod", RegexOptions.IgnoreCase))
                details.operating_system = "iOS";
            else if (Regex.IsMatch(userAgent, "linux", RegexOptions.IgnoreCase))
                details.operating_system = "Linux";
            else
                details.operating_system = "Unknown";

            // Browser
            if (Regex.IsMatch(userAgent, "edg", RegexOptions.IgnoreCase))
                details.browser = "Microsoft Edge";
            else if (Regex.IsMatch(userAgent, "opr|opera", RegexOptions.IgnoreCase))
                details.browser = "Opera";
            else if (Regex.IsMatch(userAgent, "chrome", RegexOptions.IgnoreCase) &&
                     !Regex.IsMatch(userAgent, "edg|opr", RegexOptions.IgnoreCase))
                details.browser  = "Google Chrome";
            else if (Regex.IsMatch(userAgent, "safari", RegexOptions.IgnoreCase) &&
                     !Regex.IsMatch(userAgent, "chrome|edg|opr", RegexOptions.IgnoreCase))
                details.browser = "Safari";
            else if (Regex.IsMatch(userAgent, "firefox", RegexOptions.IgnoreCase))
                details.browser = "Mozilla Firefox";
            else
                details.browser = "Unknown";

            // Location data from ipinfo.io
            try
            {
                using var client = new HttpClient();
                var response = await client.GetStringAsync("https://ipinfo.io/json");
                using var doc = JsonDocument.Parse(response);

                details.country_code = doc.RootElement.GetProperty("country").GetString();
                details.city = doc.RootElement.GetProperty("city").GetString();

                // Convert country code to name (basic fallback)
                details.country_name = details.country_code;
            }
            catch
            {
                details.country_code = details.country_code ?? "Unknown";
                details.country_name = details.country_code;
            }

            return details;
        }
    }
}
