using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace HoppsWebPlatform_Revamp.Utilities
{
    public static class IGBHelper
    {
        /// <summary>
        /// Gets the pilots corporation name from browser header variables
        /// </summary>
        /// <param name="variables">Request variables</param>
        /// <returns>Corporation name</returns>
        public static string GetCorpName(NameValueCollection variables)
        {
            return variables["HTTP_EVE_CORPNAME"];
        }

        /// <summary>
        /// Gets the pilots alliance name from the browser header variables
        /// </summary>
        /// <param name="variables">Request variables</param>
        /// <returns>Alliance name</returns>
        public static string GetAllianceName(NameValueCollection variables)
        {
            return variables["HTTP_EVE_ALLIANCENAME"];
        }

        /// <summary>
        /// Gets the pilots name from the browser header variables
        /// </summary>
        /// <param name="variables">Request variables</param>
        /// <returns>Pilot name</returns>
        public static string GetPilotName(NameValueCollection variables)
        {
            return variables["HTTP_EVE_CHARNAME"];
        }

        /// <summary>
        /// Gets a switch based on whether the pilot has the website trusted
        /// </summary>
        /// <param name="variables">Request variables</param>
        /// <returns>Switch based on whether the website is trusted</returns>
        public static bool IsWebsiteTrusted(NameValueCollection variables)
        {
            return (variables["HTTP_EVE_TRUSTED"] == "Yes");
        }

        /// <summary>
        /// Gets the pilots active ship type name (IE: Hurricane, Wolf etc.. Not ship name)
        /// </summary>
        /// <param name="variables">Request variables</param>
        /// <returns>Active ship type name</returns>
        public static string GetActiveShipTypeName(NameValueCollection variables)
        {
            return variables["HTTP_EVE_SHIPTYPENAME"];
        }

        /// <summary>
        /// Gets the pilots current solar system name
        /// </summary>
        /// <param name="variables">Request variables</param>
        /// <returns>Solar system</returns>
        public static string GetCurrentSolarSystem(NameValueCollection variables)
        {
            return variables["HTTP_EVE_SOLARSYSTEMNAME"];
        }

        /// <summary>
        /// Switch based on whether the pilot is using the IGB browser
        /// </summary>
        /// <param name="request">Base HTTP request</param>
        /// <returns>Switch based on whether the user is using the IGB browser</returns>
        public static bool IsUsingIGB(System.Web.HttpRequestBase request)
        {
            return request.UserAgent.Contains("EVE-IGB");
        }
    }
}
