using DataAccess.DtoModels.Response;
using DataAccess.Model;
using PortFolioAPI.DtoModels.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Repositories
{
    public interface IRepository
    {
        public int Add(ViewerDto viewer);
        public bool Login(Login login);
        public List<VisitorsListResponse> GetVisitors();
    }
}
