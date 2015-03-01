using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoppsWebPlatform_Revamp.Models;
using HoppsWebPlatform_Revamp.DataAccess;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;

namespace HoppsWebPlatform_Revamp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Offline()
        {
            return View("~/Views/Shared/CustomError.cshtml", (object)"ss");
        }
    }
}
