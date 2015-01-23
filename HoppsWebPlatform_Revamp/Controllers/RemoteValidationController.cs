using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoppsWebPlatform_Revamp.Models;
using HoppsWebPlatform_Revamp.DataAccess;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using System.Web.UI;
using System.Globalization;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;

namespace HoppsWebPlatform_Revamp.Controllers
{
    [OutputCache(Location=OutputCacheLocation.None, NoStore=true)]
    public class RemoteValidationController : Controller
    {
        private IApiRepository _apiRepository;
        public RemoteValidationController(IApiRepository repository)
        {
            _apiRepository = repository;
        }

        /// <summary>
        /// Remote validation attribute action that checks if the pilot is registered
        /// </summary>
        /// <param name="Username">Username to check</param>
        /// <returns>Serialized result of a bool if username is available or an error message if not</returns>
        public JsonResult IsUID_Available(string Username)
        {
            //Returns true if username is available
            if (!WebSecurity.UserExists(Username))
                return Json(true, JsonRequestBehavior.AllowGet);

            //Error message to display / parse in the serialized result
            string errorMessage = String.Format(CultureInfo.InvariantCulture,
                "{0} is not available.", Username);

            //for (int i = 1; i < 100; i++)
            //{
            //    string altCandidate = Username + i.ToString();
            //    if (!WebSecurity.UserExists(altCandidate))
            //    {
            //        suggestedUID = String.Format(CultureInfo.InvariantCulture,
            //       "{0} is not available.", Username);
            //        break;
            //    }
            //}
            return Json(errorMessage, JsonRequestBehavior.AllowGet);
        }

    }
}
