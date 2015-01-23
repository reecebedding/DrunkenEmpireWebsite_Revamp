using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using Microsoft.Practices.EnterpriseLibrary.Data;
using HoppsWebPlatform_Revamp.Models;
using System.Data.SqlClient;
using System.Data;

namespace HoppsWebPlatform_Revamp.DataAccess
{
    public class LogRepository : DbContext, ILogRepository
    {
        #region Mappers

        private IRowMapper<Models.Log> _logBaseMapper;
        private IMapBuilderContext<Models.Log> CreateMainMapper()
        {
            return MapBuilder<Models.Log>.MapAllProperties();
        }

        #endregion

        public LogRepository()
        {
            _logBaseMapper = CreateMainMapper().Build();
        }

        /// <summary>
        /// Gets a list of all log entries
        /// </summary>
        /// <returns>List of log entries</returns>
        public IEnumerable<Log> GetAllLogEntries()
        {
            IEnumerable<Models.Log> logs = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_Log_Get_All_Logs", _logBaseMapper);
            return logs;
        }
        
        /// <summary>
        /// Gets a paged list of the complete log
        /// </summary>
        /// <param name="pageNumber">Current page number. 1 = first page</param>
        /// <param name="pageSize">Quantity of logs to get</param>
        /// <param name="totalPages">Outs the total pages of the log so we can do X / Y pagination</param>
        /// <returns>Collection of logs</returns>
        public IEnumerable<Log> GetPagedLogEntries(int pageNumber, int pageSize, out int totalPages)
        {
            IEnumerable<Log> logs = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_Log_Get_Paged_Logs", _logBaseMapper, pageNumber, pageSize);
            //Get the total count of logs
            int totalLogs = (int)HoppsWebPlatformRevampDatabase.ExecuteScalar(CommandType.Text, "SELECT COUNT(*) FROM Logs");
            //Calculate whether there are still logs left after the current page
            int remainder = totalLogs % pageSize;
            //If the remainer is greater than 0, get the total division count + 1 to account for the partial remainder page, if it is 0, the total is a straight up division
            totalPages = (remainder > 0) ? (totalLogs / pageSize) + 1 : totalLogs / pageSize;
            return logs;
        }
    }
}
