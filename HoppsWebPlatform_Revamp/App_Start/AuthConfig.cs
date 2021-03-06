﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using HoppsWebPlatform_Revamp.Models;
using WebMatrix.WebData;

namespace HoppsWebPlatform_Revamp
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");

            //OAuthWebSecurity.RegisterFacebookClient(
            //    appId: "",
            //    appSecret: "");

            //OAuthWebSecurity.RegisterGoogleClient();
        }

        public static void InitSimpleMembership()
        {
            try
            {
                if (!WebSecurity.Initialized)
                {
                    WebSecurity.InitializeDatabaseConnection("LiveTest", "webpages_Users", "UserID", "UserName", autoCreateTables: false);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized.", ex);
            }
        }
    }
}
