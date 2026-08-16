using DataAccess.Dto;
using PortFolioAPI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Mappers
{
    public interface IMapper
    {
        Viewer Map(ViewerDto viewerDto);
    }
    public class Mapper : IMapper
    {
        public Viewer Map(ViewerDto viewerDto)
        {
            var viewer = new Viewer
            {
                id = viewerDto.id,
                country_code = viewerDto.country_code,
                country_name = viewerDto.country_name,
                city = viewerDto.city,
                timezone = viewerDto.timezone,
                device_type = viewerDto.device_type,
                operating_system = viewerDto.operating_system,
                browser = viewerDto.browser,
                user_agent = viewerDto.user_agent,
                page_url = viewerDto.page_url,
                referrer = viewerDto.referrer,
                visit_time = viewerDto.visit_time
            };
            return viewer;
        }
    }
}
