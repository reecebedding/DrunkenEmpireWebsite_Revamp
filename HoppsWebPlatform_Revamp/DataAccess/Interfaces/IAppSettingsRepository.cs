using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoppsWebPlatform_Revamp.Models;

namespace HoppsWebPlatform_Revamp.DataAccess.Interfaces
{
    public interface IAppSettingsRepository
    {
        ApiKey GetDirectorAPI();
        ApiKey GetCorporationAPI();
        AppSetting GetSettingByName(string settingName);
        bool UpdateSettingByName(string settingName, string value, out int result);
        bool UpdateDirectorAPIKey(ApiKey directorAPI, out int result);
        bool UpdateCorporationAPIKey(ApiKey corporationAPI, out int result);
    }
}
