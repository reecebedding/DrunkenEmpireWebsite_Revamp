using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoppsWebPlatform_Revamp.Models;

namespace HoppsWebPlatform_Revamp.DataAccess.Interfaces
{
    public interface IApiRepository
    {
        IEnumerable<ApiKey> GetAllApisForPilot(string pilotName);
        bool AddAPIKey(ApiKey newAPIKey, out int resultVal);
        IEnumerable<ApiKey> GetAllAPIByKeyIDAndVCode(long keyID, string vCode);
        ApiKey GetAPIByID(int uid);
        bool UpdateAPIKey(ApiKey oldAPI, ApiKey newAPI);
    }
}
