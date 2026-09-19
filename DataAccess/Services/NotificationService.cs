using DataAccess.Model;
using DataAccess.Models.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PortFolioAPI.DataAccess;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace DataAccess.Services
{
    public interface INotificationService
    {
        Task<string> SendEmailByBrevo(string subject, string htmlContent, string textContent);
        Task<string> SendWhatsppMessageByTwilio(string message);
        Task<string> FetchTwilioBalance();
        Task<string> GetBrevoEmailBalance();
        Task<int> GetBrevoEmailBalanceAsync();
    }
    public class NotificationService : INotificationService
    {
        private static readonly HttpClient _httpClient = new();
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificationService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task<string> FetchTwilioBalance()
        {
            var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            TwilioClient.Init(accountSid, authToken);
            var balance = Twilio.Rest.Api.V2010.Account.BalanceResource.Fetch();
            return Task.FromResult($"{balance.Balance} {balance.Currency}");
        }

        public async Task<string> SendEmailByBrevo(string subject, string htmlContent, string textContent)
        {
            var apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY");
            var senderEmail = Environment.GetEnvironmentVariable("BREVO_SENDER_EMAIL");
            var destinationEmail = Environment.GetEnvironmentVariable("BREVO_DESTINATION_EMAIL");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/smtp/email");
                request.Headers.Add("api-key", apiKey);
                request.Content = JsonContent.Create(new
                {
                    sender = new { name = "Portfolio Alerts", email = senderEmail },
                    to = new[] { new { email = destinationEmail } },
                    subject,
                    htmlContent,
                    textContent
                });

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Status: {(int)response.StatusCode}");
                Console.WriteLine($"Response: {responseBody}");

                // Save log entry
                var log = new BrevoMailLogs
                {
                    subject = subject,
                    htmlContent = htmlContent,
                    textContent = textContent,
                    senderEmail = senderEmail,
                    destinationEmail = destinationEmail,
                    StatusCode = (int)response.StatusCode,
                    ResponseBody = responseBody,
                    ErrorMessage = response.IsSuccessStatusCode ? null : "Brevo API returned error",
                    CreatedAt = DateTime.UtcNow
                };

                // Example: using EF Core
                await SaveMailLog(log);

                return response.IsSuccessStatusCode ? "Sent" : $"Failed: {responseBody}";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Brevo Email Exception: {ex}");

                var log = new BrevoMailLogs
                {
                    subject = subject,
                    htmlContent = htmlContent,
                    textContent = textContent,
                    senderEmail = senderEmail,
                    destinationEmail = destinationEmail,
                    StatusCode = 0, // no response
                    ResponseBody = null,
                    ErrorMessage = ex.Message,
                    CreatedAt = DateTime.UtcNow
                };

                await SaveMailLog(log);

                return $"Error: {ex.Message}";
            }
        }

        private async Task SaveMailLog(BrevoMailLogs log)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                dbContext.brevo_mail_logs.Add(log);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Mail log save failed: {ex.Message}");
            }
        }


        //public async Task<MessageResponse> SendNotification(string usermessage)
        //{
        //    // Implementation for sending notification
        //    using var client = new HttpClient();

        //    var message = new StringContent(usermessage, Encoding.UTF8);
        //    message.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

        //    var response = await client.PostAsync("https://ntfy.sh/yasershaikhportfoliovisitors327", message);
        //    var responseText = await response.Content.ReadAsStringAsync();

        //    var statusCode = response.StatusCode;

        //    var Response = new MessageResponse
        //    {
        //        status = statusCode.ToString(),
        //        response = responseText
        //    };

        //    return Response;
        //}
        public Task<string> SendWhatsppMessageByTwilio(string message)
        {
            var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
            var sourcePhoneNumber = Environment.GetEnvironmentVariable("TWILIO_SOURCE_PHONE_NUMBER");
            var destinationPhoneNumber = Environment.GetEnvironmentVariable("TWILIO_DESTINATION_PHONE_NUMBER");

            TwilioClient.Init(accountSid, authToken);

            var messageBody = MessageResource.Create(
                body: message,
                from: new PhoneNumber(sourcePhoneNumber),
                to: new PhoneNumber(destinationPhoneNumber)
            );

            Console.WriteLine($"Message SID: {messageBody.Sid}");
            Console.WriteLine($"Status: {messageBody.Status}");
            return Task.FromResult(messageBody.Status.ToString());
        }

        public async Task<string> GetBrevoEmailBalance()
        {
            var apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY");

            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.brevo.com/v3/account");
            request.Headers.Add("api-key", apiKey);
            request.Headers.Add("accept", "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Brevo account call failed: {(int)response.StatusCode} {responseBody}");
                return $"Failed: {responseBody}";
            }

            using var doc = JsonDocument.Parse(responseBody);

            if (!doc.RootElement.TryGetProperty("plan", out var plans))
                return "No plan information returned.";

            var lines = new List<string>();

            foreach (var plan in plans.EnumerateArray())
            {
                var type = plan.TryGetProperty("type", out var t) ? t.GetString() : "unknown";
                var creditsType = plan.TryGetProperty("creditsType", out var ct) ? ct.GetString() : "-";
                var credits = plan.TryGetProperty("credits", out var c) ? c.GetDouble() : 0;

                lines.Add($"{type} ({creditsType}): {credits}");
            }

            var result = string.Join(Environment.NewLine, lines);
            Console.WriteLine(result);
            return result;
        }

        public async Task<int> GetBrevoEmailBalanceAsync()
        {
            var apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY");

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", apiKey);

            var response = await client.GetStringAsync("https://api.brevo.com/v3/account");

            using var doc = JsonDocument.Parse(response);

            // Access the first item in the "plan" array
            int credits = doc.RootElement
                .GetProperty("plan")[0]
                .GetProperty("credits")
                .GetInt32();

            return credits;


        }

    }
    }
