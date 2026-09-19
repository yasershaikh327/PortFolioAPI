using DataAccess.DtoModels.Response;
using DataAccess.Helper;
using DataAccess.Mappers;
using DataAccess.Model;
using DataAccess.Models;
using DataAccess.Services;
using DataAccess.Templates;
using Microsoft.EntityFrameworkCore;
using PortFolioAPI.DataAccess;
using PortFolioAPI.DtoModels.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repositories
{
    public class Repository : IRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly INotificationService _iNotificationService;
        private readonly IHelper _helper;
        public Repository(IMapper mapper, ApplicationDbContext applicationDbContext, INotificationService iNotificationService, IHelper helper)
        {
            _mapper = mapper;
            _applicationDbContext = applicationDbContext;
            _iNotificationService = iNotificationService;
            _helper = helper;
        }
        public async Task<int> Add(ViewerDto viewer)
        {
            try
            {
                // Map DTO to entity
                var viewerList = _mapper.Map(viewer);

                // If timezone is provided, convert UTC to local time
                if (!string.IsNullOrEmpty(viewerList.timezone))
                {
                    try
                    {
                        var tz = TimeZoneInfo.FindSystemTimeZoneById(viewerList.timezone);
                        var localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);

                        // Ensure EF/Npgsql sees this as UTC
                        viewerList.visit_time = DateTime.SpecifyKind(localTime, DateTimeKind.Utc);
                    }
                    catch (TimeZoneNotFoundException)
                    {
                        viewerList.visit_time = DateTime.UtcNow;
                    }
                }
                else
                {
                    viewerList.visit_time = DateTime.UtcNow;
                }

                // Always enforce UTC kind
                viewerList.visit_time = DateTime.SpecifyKind(viewerList.visit_time, DateTimeKind.Utc);

                //Local Indian Timing
                var LocalTime = DateTime.Now;

                //Notification
                var buildVisitorAlertHtml = new BrevoMailTemplate();
                var html = buildVisitorAlertHtml.BuildVisitorAlertHtml(viewer.city, viewer.country_name, LocalTime.ToString("yyyy-MM-dd HH:mm:ss"), viewer.browser, viewer.operating_system);

                var text = $"👀 Visitor Alert: Location 📍 {viewer.city}, {viewer.country_name}; " +
                           $"Time 🕐 {LocalTime}; Browser 🌐 {viewer.browser}; OS 💻 {viewer.operating_system}";

                int balance = await _iNotificationService.GetBrevoEmailBalanceAsync();

                if (balance <= 50)
                {
                    string htmlBody = $@"
                        <div style='font-family:Arial, sans-serif; background-color:#f9f9f9; padding:20px; border:1px solid #ddd; border-radius:8px;'>
                            <h1 style='color:#d9534f; text-align:center;'>⚠️ Low Balance Alert</h1>
                            <p style='font-size:16px; color:#333; text-align:center;'>
                                Your Brevo email credits are running low.<br/>
                                <strong style='color:#d9534f;'>Remaining Credits: {balance}</strong>
                            </p>
                        </div>";

                    string textBody = $"Low Balance Alert - Remaining Credits: {balance}";

                    await _iNotificationService.SendEmailByBrevo(
                        "👀 Visitor Alert",
                        htmlBody,
                        textBody
                    );
                }
                else
                {
                    await _iNotificationService.SendEmailByBrevo("👀 Visitor Alert", html, text);
                }

                // Insert into database
                _applicationDbContext.viewers_list.Add(viewerList);
                _applicationDbContext.SaveChanges();

                // Return total records count
                var totalRecords = _applicationDbContext.viewers_list.Count();
                return totalRecords;
            }
            catch (DbUpdateException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<VisitorsListResponse> GetVisitors()
        {
            var visitors = _applicationDbContext.viewers_list.ToList();

            var visitorsResponse = new List<VisitorsListResponse>();

            foreach (var visitObject in visitors)
            {
                visitorsResponse.Add(new VisitorsListResponse
                {
                    id = visitObject.id,
                    country_code = visitObject.country_code,
                    country_name = visitObject.country_name,
                    city = visitObject.city,
                    timezone = visitObject.timezone,
                    device_type = visitObject.device_type,
                    operating_system = visitObject.operating_system,
                    browser = visitObject.browser,
                    page_url = visitObject.page_url,
                    referrer = visitObject.referrer,
                    visit_time = _helper.ConvertUtcToIndiaTime(visitObject.visit_time) // your conversion helper
                });
            }

            return visitorsResponse;
        }


        public bool Login(Login login)
        {
            try
            {
                if(login.Email == "syaser327@gmail.com" && login.Password == "Admin#789")
                {
                    return true;
                }
                return false;
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
