using DataAccess.Model;
using DataAccess.Models;
using PortFolioAPI.DtoModels.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Mappers
{
    public interface IMapper
    {
        Viewer Map(ViewerDto viewerDto);
    }
}
