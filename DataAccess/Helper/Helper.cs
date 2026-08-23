using DataAccess.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DataAccess.Helper
{
    public class Helper : IHelper
    {
        private readonly IConfiguration _config;
        public Helper(IConfiguration config)
        {
            _config = config;
        }   
        public async Task LogError(string message, Exception ex = null)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "An error occurred while processing the request.";
            }

            // Get project root (one level up from bin folder)
            string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));

            // Create Logs folder inside project root
            string folderPath = Path.Combine(projectRoot, "Logs");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Daily log file name
            string filePath = Path.Combine(folderPath, $"error_{DateTime.Now:yyyyMMdd}.txt");

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                writer.WriteLine("Error Message: " + message);

                if (ex != null)
                {
                    writer.WriteLine("Exception: " + ex.Message);
                    writer.WriteLine("Stack Trace: " + ex.StackTrace);
                }

                writer.WriteLine(new string('-', 50));
            }
        }

        public string GenerateJwtToken(string username)
        {
            var jwtSettings = _config.GetSection("JwtSettings").Get<JwtSettings>();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username)
        };

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,        // must match middleware ValidIssuer
                audience: jwtSettings.Audience,    // must match middleware ValidAudience
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public DateTime ConvertUtcToIndiaTime(DateTime utcDateTime)
        {
            if (utcDateTime.Kind == DateTimeKind.Unspecified)
                utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

            TimeZoneInfo indiaZone;

            if (OperatingSystem.IsWindows())
                indiaZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            else
                indiaZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, indiaZone);
        }
    }
}