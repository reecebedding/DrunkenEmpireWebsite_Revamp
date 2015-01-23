using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.Data;
using HoppsWebPlatform_Revamp.Models;
using HoppsWebPlatform_Revamp.Utilities;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;

namespace HoppsWebPlatform_Revamp.DataAccess
{
    public class AppSettingsRepository : DbContext, IAppSettingsRepository
    {
        #region Properties

        private NLog.Logger _logger;
        private IRowMapper<AppSetting> _rowMapperMain;
        private IMapBuilderContext<AppSetting> CreateMainMapper()
        {
            return MapBuilder<AppSetting>.MapAllProperties();
        }
        
        #endregion

        #region Constructor

        public AppSettingsRepository()
        {
            _rowMapperMain = CreateMainMapper().Build();
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        #endregion

        /// <summary>
        /// Gets an app setting based on setting name
        /// </summary>
        /// <param name="settingName">Setting name</param>
        /// <returns>App settings</returns>
        public AppSetting GetSettingByName(string settingName)
        {
            AppSetting setting = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_App_Settings_Get_Setting_By_Name", _rowMapperMain, settingName).SingleOrDefault();
            return setting;
        }

        /// <summary>
        /// Updates a setting in the app setting table to a new value
        /// </summary>
        /// <param name="settingName">Name of setting to set the value for</param>
        /// <param name="value">Value to set the setting to</param>
        /// <param name="result">Integer value representing the transaction result</param>
        /// <returns>Switch based on whether the setting was correctly updated</returns>
        public bool UpdateSettingByName(string settingName, string value, out int result)
        {
            result = -1;
            try
            {
                result = (int)HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_App_Settings_Update_Setting_By_Name", settingName, value);
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst attempting to changed app setting : {0} to value : {1}. Exception : {2}", settingName, value, exn.Message));
                return false;
            }
            
            if (result == 1)
            {
                _logger.Info(string.Format("Setting : {0} was updated to : {1}", settingName, value));
                return true;
            }
            else
                _logger.Warn(string.Format("Setting : {0} was unable to be set to : {1}", settingName, value));

            return false;
        }

        /// <summary>
        /// Gets the full director API Key from app settings
        /// </summary>
        /// <returns>Full director API Key</returns>
        public ApiKey GetDirectorAPI()
        {
            long keyID = Convert.ToInt64(GetSettingByName("DirectorKeyID").Value);
            string vCode = GetSettingByName("DirectorVCode").Value;
            return new ApiKey()
            {
                KeyID = keyID,
                VCode = vCode,
                Valid = APIHelper.EVE_IsAPIValid(keyID, vCode)
            };
        }
          
        /// <summary>
        /// Gets the full corporation API Key from app settings
        /// </summary>
        /// <returns>Full corporation API key</returns>
        public ApiKey GetCorporationAPI()
        {
            long keyID = Convert.ToInt64(GetSettingByName("CorporationKeyID").Value);
            string vCode = GetSettingByName("CorporationVCode").Value;
            return new ApiKey()
            {
                KeyID = keyID,
                VCode = vCode,
                Valid = APIHelper.EVE_IsAPIValid(keyID, vCode)
            };
        }

        /// <summary>
        /// Updates the director API key
        /// </summary>
        /// <param name="directorAPI">Director API to update.</param>
        public bool UpdateDirectorAPIKey(ApiKey directorAPI, out int result)
        {
            result = (int)HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_App_Settings_Update_Director_API", directorAPI.KeyID, directorAPI.VCode);
            if (result == 1)
            {
                _logger.Info(string.Format("Director API was successfully updated to: KeyID: {0}, VCode: {1}", directorAPI.KeyID, directorAPI.VCode));
                return true;
            }
            else if (result != 2)
                _logger.Warn(string.Format("Director API was unable to be updated to: KeyID: {0}, VCode: {1}", directorAPI.KeyID, directorAPI.VCode));
            return false;
        }
        
        /// <summary>
        /// Updates the corporation API key
        /// </summary>
        /// <param name="corporationAPI">Corporation API to update.</param>
        public bool UpdateCorporationAPIKey(ApiKey corporationAPI, out int result)
        {
            result = (int)HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_App_Settings_Update_Corporation_API", corporationAPI.KeyID, corporationAPI.VCode);
            if (result == 1)
            {
                _logger.Info(string.Format("Corporation API was successfully updated to: KeyID: {0}, VCode: {1}", corporationAPI.KeyID, corporationAPI.VCode));
                return true;
            }
            else if (result != 2)
                _logger.Warn(string.Format("Corporation API was unable to be updated to: KeyID: {0}, VCode: {1}", corporationAPI.KeyID, corporationAPI.VCode));
            return false;
        }
    }
}