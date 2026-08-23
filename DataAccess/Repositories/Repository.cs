using DataAccess.DtoModels.Response;
using DataAccess.Helper;
using DataAccess.Mappers;
using DataAccess.Model;
using DataAccess.Models;
using DataAccess.Services;
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
        public int Add(ViewerDto viewer)
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
                _iNotificationService.SendNotification($"👀 Visitor Alert: Location 📍 {viewer.city}, {viewer.country_name}; " + $"Time 🕐 {LocalTime.ToString("dd/MM/yyyy hh:mm tt")}; " + $"Browser 🌐 {viewer.browser}; OS 💻 {viewer .operating_system}");

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
