using DataAccess.Dto;
using DataAccess.Mappers;
using Microsoft.EntityFrameworkCore;
using PortFolioAPI.DataAccess;
using PortFolioAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repositories
{
    public interface IRepository
    {
        public int Add(ViewerDto viewer);
    }
    public class Repository : IRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _applicationDbContext;

        public Repository(IMapper mapper, ApplicationDbContext applicationDbContext)
        {
            _mapper = mapper;
            _applicationDbContext = applicationDbContext;
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

    }
}
