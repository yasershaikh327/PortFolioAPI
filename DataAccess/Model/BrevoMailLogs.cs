using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Model
{
    public class BrevoMailLogs
    {
        public int id { get; set; }                // Primary key
        public string subject { get; set; }        // Email subject
        public string htmlContent { get; set; }    // HTML body
        public string textContent { get; set; }    // Plain text body
        public string senderEmail { get; set; }    // Who sent
        public string destinationEmail { get; set; } // Who received
        public int StatusCode { get; set; }        // HTTP status code
        public string? ResponseBody { get; set; }   // Raw API response
        public string? ErrorMessage { get; set; }   // Exception message if any
        public DateTime CreatedAt { get; set; }    // Timestamp
    }

}
