using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HoppsWebPlatform_Revamp.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;

namespace HoppsWebPlatform_Revamp.DataAccess
{
    public class AltRepository : DbContext, IAltRepository
    {
        #region Properties

        private NLog.Logger _logger;

        #endregion

        #region Mappers

        private IRowMapper<Alt> _rowMapperMain;
        private IMapBuilderContext<Alt> CreateMainMapper()
        {
            return MapBuilder<Alt>.MapAllProperties();
        }

        #endregion

        /// <summary>
        /// Constructor for repository
        /// </summary>
        public AltRepository()
        {
            _rowMapperMain = CreateMainMapper().Build();
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Gets all alts belonging to the pilot even if its an alt.
        /// </summary>
        /// <param name="pilotName">Pilot name</param>
        /// <returns>List of alts</returns>
        public IEnumerable<Alt> GetAllAltsForPilot(string pilotName)
        {
            IEnumerable<Alt> alts = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_Alt_Get_All_Alts_For_Pilot", _rowMapperMain, pilotName);
            return alts;          
        }

        /// <summary>
        /// Adds an alt to the alt list
        /// </summary>
        /// <param name="newAlt">Alt to add to the alt list</param>
        /// <returns>Bool based on whether the alt was succesfully added</returns>
        public bool AddAlt(Alt newAlt)
        {
            int result = (int)HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_Alt_Add_Alt", newAlt.MainName, newAlt.AltName, newAlt.AltPurpose);
            if (result == 1)
            {
                _logger.Info(string.Format("Alt: {0} for main: {1} was successfully added.", newAlt.AltName, newAlt.MainName));
                return true;
            }
            _logger.Warn(string.Format("Alt: {0} for main: {1} was unable to be added.", newAlt.AltName, newAlt.MainName));
            return false;
        }

        /// <summary>
        /// Gets a specific alt from the database by alt name
        /// </summary>
        /// <param name="altName">Alt name to get</param>
        /// <returns>Alt</returns>
        public Alt GetAlt(string altName)
        {
            var alts = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_Alt_Get_Alt", _rowMapperMain, altName);
            if (alts.Count() > 0)
                return alts.First();
            else
                return null;
        }
    }
}