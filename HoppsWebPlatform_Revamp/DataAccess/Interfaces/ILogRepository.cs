using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoppsWebPlatform_Revamp.Models;

namespace HoppsWebPlatform_Revamp.DataAccess.Interfaces
{
    public interface ILogRepository
    {
        IEnumerable<Log> GetAllLogEntries();
        IEnumerable<Log> GetPagedLogEntries(int pageNumber, int pageCount, out int totalPages);
    }
}
