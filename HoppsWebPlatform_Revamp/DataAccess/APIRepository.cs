using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using HoppsWebPlatform_Revamp.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;

namespace HoppsWebPlatform_Revamp.DataAccess
{
    public class APIRepository : DbContext, IApiRepository
    {
        #region Properties

        private NLog.Logger _logger;

        #endregion

        #region Mappers

        private IRowMapper<ApiKey> _rowMapperMain;
        private IMapBuilderContext<ApiKey> CreateMainMapper()
        {
            return MapBuilder<ApiKey>.MapAllProperties();
        }

        #endregion

        #region Constructor

        public APIRepository()
        {
            _rowMapperMain = CreateMainMapper().Build();
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        #endregion

        /// <summary>
        /// Gets all the APIS Associated with the pilot
        /// </summary>
        /// <param name="pilotName">Pilot name to get all associated API'S</param>
        /// <returns>List of api keys for pilot</returns>
        public IEnumerable<Models.ApiKey> GetAllApisForPilot(string pilotName)
        {
            IEnumerable<ApiKey> apis = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor<ApiKey>("Web_Api_Get_All_Apis_For_Pilot", _rowMapperMain, new object[] { pilotName });
            return apis;           
        }

        /// <summary>
        /// Adds a new API Key
        /// </summary>
        /// <param name="newAPIKey">New API Key</param>
        /// <returns>Switch based on whether the insert was succesful. And outs result val - 1 = Successful, 2 = API Exists 0 = Failure</returns>
        public bool AddAPIKey(ApiKey newAPIKey, out int resultVal)
        {
            resultVal = (int)HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_Api_Add_Api", newAPIKey.KeyID, newAPIKey.VCode, newAPIKey.PilotName);
            if (resultVal == 1)
            {
                _logger.Info(string.Format("API - KeyID: {0}, KeyID: {1} was successfully addded.", newAPIKey.KeyID, newAPIKey.VCode));
                return true;
            }
            _logger.Warn(string.Format("New API key - KeyID: {0}, VCode: {1} was unable to be added.", newAPIKey.KeyID, newAPIKey.VCode));
            return false;
        }

        /// <summary>
        /// Gets all API keys with the keyID and vCode parameters
        /// </summary>
        /// <param name="keyID">API KeyID</param>
        /// <param name="vCode">API VCode</param>
        /// <returns>List of API's matching </returns>
        public IEnumerable<ApiKey> GetAllAPIByKeyIDAndVCode(long keyID, string vCode)
        {
            IEnumerable<ApiKey> apiKeys = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_Api_Get_Api_By_KeyID_And_VCode", _rowMapperMain, keyID, vCode);
            return apiKeys;
        }

        /// <summary>
        /// Gets an API by Unique ID
        /// </summary>
        /// <param name="uid">API UID</param>
        /// <returns>API</returns>
        public ApiKey GetAPIByID(int uid)
        {
            ApiKey key = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_Api_Get_Api_By_ID", _rowMapperMain, uid).SingleOrDefault();
            return key;
        }

        /// <summary>
        /// Updates all APIS's matching the oldAPI values with the newAPI values.
        /// </summary>
        /// <param name="oldAPI">Old API</param>
        /// <param name="newAPI">New API</param>
        /// <returns>Switch based on whether the update was succesful</returns>
        public bool UpdateAPIKey(ApiKey oldAPI, ApiKey newAPI)
        {
            int result = (int)HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_Api_Update", oldAPI.KeyID, oldAPI.VCode, newAPI.KeyID, newAPI.VCode);
            if (result == 1)
            {
                _logger.Info(string.Format("User successfully updated their API Key To: KeyID: {0} VCode: {1}", newAPI.KeyID, newAPI.VCode));
                return true;
            }
            _logger.Warn(string.Format("API key - KeyID: {0}, VCode: {1} was unable to be updated."));
            return false;
        }
    }
}
