using DataAccess.Models.Response;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace DataAccess.Services
{
    public interface INotificationService
    {
        Task<string> SendNotification(string message);
    }
    public class NotificationService : INotificationService
    {
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
        public Task<string> SendNotification(string message)
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
    }
}
