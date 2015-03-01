using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HoppsWebPlatform_Revamp.Models;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace HoppsWebPlatform_Revamp.Attributes
{
    public class Authorize : AuthorizeAttribute
    {
        public string Roles { get; set; }
        private ApplicationUserManager _userManager;

        public Authorize()
        {
            object resolvedObject = System.Web.Mvc.DependencyResolver.Current.GetService(typeof(ApplicationUserManager));
            if (resolvedObject != null)
                _userManager = (ApplicationUserManager)resolvedObject;  
        }


        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (Roles == null)
            {
                return httpContext.User.Identity.Name != null;
            }

            string[] roles = Roles.Split(',');

            ApplicationUser user = _userManager.FindByName(httpContext.User.Identity.Name);

            if (user != null)
            {                
                foreach (string role in roles)
                {                    
                    if (_userManager.IsInRole(user.Id, role.Trim()))
                        return true;
                }
            }
            return false;
        }
    }
}