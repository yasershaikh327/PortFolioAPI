using DataAccess.Models.Response;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace DataAccess.Services
{
    public interface INotificationService
    {
        Task<MessageResponse> SendNotification(string message);
    }
    public class NotificationService : INotificationService
    {
        public async Task<MessageResponse> SendNotification(string usermessage)
        {
            // Implementation for sending notification
            using var client = new HttpClient();

            var message = new StringContent(usermessage, Encoding.UTF8);
            message.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

            var response = await client.PostAsync("https://ntfy.sh/yasershaikhportfoliovisitors327", message);
            var responseText = await response.Content.ReadAsStringAsync();

            var statusCode = response.StatusCode;

            var Response = new MessageResponse
            {
                status = statusCode.ToString(),
                response = responseText
            };

            return Response;
        }
    }
}
