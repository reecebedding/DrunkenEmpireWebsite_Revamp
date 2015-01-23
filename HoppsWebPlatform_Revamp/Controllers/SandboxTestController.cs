using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoppsWebPlatform_Revamp.Models;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using HoppsWebPlatform_Revamp.DataAccess;
using HoppsWebPlatform_Revamp.Utilities;

namespace HoppsWebPlatform_Revamp.Controllers
{
    public class SandboxTestController : Controller
    {
        //
        // GET: /SandboxTest/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RCBlackList()
        {
            return View();
        }

        [HttpPost]
        public bool RCBlackList(string pilot)
        {
            return HoppsWebPlatform_Revamp.Utilities.APIHelper.RC_IsPilotBlacklisted(pilot);
        }

        public ActionResult EmailTest()
        {
            return View();
        }

        [HttpPost]
        public bool EmailTest(string email)
        {
            try 
	        {	        
		        Utilities.EmailHelper.SendEmail(Utilities.EmailHelper.RecruitmentEmail, email, "Test", "<b>Test</b>", false);
                return true;
	        }
	        catch (Exception)
	        {
		        return false;
	        }
            
        }
        public ActionResult MigratedDesign()
        {
            return View();
        }


    }
}
