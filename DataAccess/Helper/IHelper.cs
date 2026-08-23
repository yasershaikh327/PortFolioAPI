using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Helper
{
    public interface IHelper
    {
        public Task LogError(string message, Exception ex = null);
        public string GenerateJwtToken(string username);
        public DateTime ConvertUtcToIndiaTime(DateTime utcDateTime);
    }
}
