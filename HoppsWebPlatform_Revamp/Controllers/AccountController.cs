﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using HoppsWebPlatform_Revamp.Filters;
using HoppsWebPlatform_Revamp.Models;
using HoppsWebPlatform_Revamp.Utilities;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using HoppsWebPlatform_Revamp.DataAccess;
using System.Net;
using System.Text;
using System.IO;
using eZet.EveLib.EveAuthModule;

namespace HoppsWebPlatform_Revamp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Properties
        private IApiRepository _apiRepository;
        private IAltRepository _altRepository;
        private NLog.Logger _logger;
        #endregion

        public AccountController(IApiRepository apiRepository, IAltRepository altRepositor)
        {
            _apiRepository = apiRepository;
            _altRepository = altRepositor;
            _logger = new NLog.LogFactory().GetCurrentClassLogger();
        }

        [AllowAnonymous]
        public ActionResult EVERegister(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {                    
                        //WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { Email = model.Email });
                        WebSecurity.CreateUserAndAccount(model.UserName, new Guid().ToString());
                        _logger.Info(string.Format("User successfully registered"));

                        //If user is in the corporation when they register, add them to the corpmember group.
                        if (APIHelper.EVE_GetPilotsCorporationID(model.UserName) == 98038363)
                        {
                            Roles.AddUserToRole(model.UserName, "CorporationMember");
                            _logger.Info(string.Format("User was added to the CorpMember group"));
                        }
                        else
                        {
                            Roles.AddUserToRole(model.UserName, "Guest");
                            _logger.Info(string.Format("User was added to the Guest group"));
                        }

                        FormsAuthentication.SetAuthCookie(model.UserName, true);
                        //WebSecurity.Login(model.UserName, model.Password);

                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                    _logger.Error(string.Format("An error occured whilst attempting to register: EXN: {0}", e.Message));
                }
            }
            //return View(new RegisterModel() { UserName = IGBHelper.GetPilotName(Request.ServerVariables) });
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult EVELogin()
        {
            string URL = "https://login.eveonline.com";
            string path = "/oauth/authorize";
            string clientID = System.Configuration.ConfigurationManager.AppSettings["EVESSOClientID"].ToString();
            string secret = System.Configuration.ConfigurationManager.AppSettings["EVESSOSecret"].ToString();
            string callbackURL = System.Configuration.ConfigurationManager.AppSettings["EVESSOCallBackURL"].ToString();

            string finalURL = string.Format("{0}{1}?response_type=code&redirect_uri={2}&client_id={3}&scope=&state=12345", URL, path, callbackURL, clientID);

            return Redirect(finalURL);
        }

        [AllowAnonymous]
        public ActionResult CallbackLogin(string code)
        {
            try
            {
                AuthResponse token = GetCharacterSSOToken(code);
                VerifyResponse characterDetails = GetCharacterIDFromToken(token);

                IEnumerable<Alt> alts = _altRepository.GetAllAltsForPilot(characterDetails.CharacterName);
                string main = characterDetails.CharacterName;
                if (alts.Count() > 0)
                {
                    main = alts.FirstOrDefault().MainName;    
                }
                


                if (WebSecurity.UserExists(main))
                    FormsAuthentication.SetAuthCookie(main, true);
                else
                    EVERegister(new RegisterModel() { UserName = main });
            
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("Unable to externally login: {0}, Inner: {1}", exn.Message, exn.InnerException));
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        private VerifyResponse GetCharacterIDFromToken(AuthResponse token)
        {
            string URL = "https://login.eveonline.com";
            string path = "/oauth/verify";
            string clientID = System.Configuration.ConfigurationManager.AppSettings["EVESSOClientID"].ToString();
            string secret = System.Configuration.ConfigurationManager.AppSettings["EVESSOSecret"].ToString();
            string callbackURL = System.Configuration.ConfigurationManager.AppSettings["EVESSOCallBackURL"].ToString();

            string finalURL = string.Format("{0}{1}", URL, path);
            string authHeader = "Bearer " + token.AccessToken;

            HttpWebRequest request = WebRequest.CreateHttp(finalURL);
            request.Host = "login.eveonline.com";
            request.UserAgent = "SSO Integration Test, HOPPS DEVELOPER WEB APPLICATION, Client ID: " + clientID;

            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";

            string responseData = "";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseData = reader.ReadToEnd();
                    }
                }
            }
            VerifyResponse responseValue = Newtonsoft.Json.JsonConvert.DeserializeObject<VerifyResponse>(responseData);
            return responseValue;
        }

        private AuthResponse GetCharacterSSOToken(string code)
        {
            string URL = "https://login.eveonline.com";
            string path = "/oauth/token";
            string clientID = System.Configuration.ConfigurationManager.AppSettings["EVESSOClientID"].ToString();
            string secret = System.Configuration.ConfigurationManager.AppSettings["EVESSOSecret"].ToString();
            string callbackURL = System.Configuration.ConfigurationManager.AppSettings["EVESSOCallBackURL"].ToString();

            string finalURL = string.Format("{0}{1}", URL, path);


            HttpWebRequest request = WebRequest.CreateHttp(finalURL);
            request.Host = "login.eveonline.com";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "SSO Integration Test, HOPPS DEVELOPER WEB APPLICATION, Client ID: " + clientID;
            
            request.Headers.Add("Authorization", "Basic " + Base64EncodeAuthString(clientID + ":" + secret));
            request.Method = "POST";

            string postData = "grant_type=authorization_code&code=" + code;
            request.ContentLength = postData.Length;
            using (var writer = new StreamWriter(request.GetRequestStream()))
                writer.Write(postData);

            string responseData = "";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseData = reader.ReadToEnd();
                    }
                }
            }
            AuthResponse responseValue = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthResponse>(responseData);
            return responseValue;
        }

        private string Base64EncodeAuthString(string value)
        {
            var plaintTextBytes = System.Text.Encoding.UTF8.GetBytes(value);
            return System.Convert.ToBase64String(plaintTextBytes);
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //return RedirectToAction("EVELogin");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult Unauthorized(string returnUrl)
        {
            ViewBag.message = "Unauthorized. Please contact a director!";
            return View("Login");
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            return RedirectToAction("EVELogin");

            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                Session["pilotID"] = Utilities.APIHelper.EVE_GetPilotIDByName(model.UserName);
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new RegisterModel() { UserName = IGBHelper.GetPilotName(Request.ServerVariables) });
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {

                    bool isAPIValid = false;

                    //Attempt to get characters on API, can cause exception for general error or Invalid API. 
                    try
                    {
                        isAPIValid = APIHelper.EVE_IsCharacterOnAPI(model.UserName, model.KeyID, model.VCode);
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "Unable to retrieve character from API. Please check API and try again.");
                    }

                    //If character exists on API, use can register.
                    if (isAPIValid)
                    {
                        int apiAddVal = 0;
                        _apiRepository.AddAPIKey(new ApiKey() { KeyID = model.KeyID, VCode = model.VCode, PilotName = model.UserName }, out apiAddVal);
                        WebSecurity.CreateUserAndAccount(model.UserName, model.Password, new { Email = model.Email });
                        _logger.Info(string.Format("User successfully registered"));

                        //If user is in the corporation when they register, add them to the corpmember group.
                        if (APIHelper.EVE_GetPilotsCorporationID(model.UserName) == 98038363)
                        {
                            Roles.AddUserToRole(model.UserName, "CorporationMember");
                            _logger.Info(string.Format("User was added to the CorpMember group"));
                        }
                        else
                        {
                            Roles.AddUserToRole(model.UserName, "Guest");
                            _logger.Info(string.Format("User was added to the Guest group"));
                        }

                        WebSecurity.Login(model.UserName, model.Password);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                        ModelState.AddModelError("", "Pilot not found on API");

                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                    _logger.Error(string.Format("An error occured whilst attempting to register: EXN: {0}", e.Message));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (UsersContext db = new UsersContext())
                {
                    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
                    if (user == null)
                    {
                        // Insert name into the profile table
                        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        /// <summary>
        /// Gets the current users Profile
        /// </summary>
        /// <returns>MyProfile view</returns>
        [Authorize]
        public ActionResult MyProfile()
        {
            return View("Partials/_PilotProfile", GetPilotsProfile(User.Identity.Name));
        }

        /// <summary>
        /// Gets a pilots Profile
        /// </summary>
        /// <param name="pilotName">Pilots name</param>
        /// <returns>Pilots profile</returns>
        [Authorize]
        private PilotProfileViewModel GetPilotsProfile(string pilotName)
        {
            try
            {
                PilotProfileViewModel model = new PilotProfileViewModel(pilotName)
                {
                    PilotID = APIHelper.EVE_GetPilotIDByName(pilotName),
                    ApiKeys = _apiRepository.GetAllApisForPilot(pilotName)
                };
                return model;
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst attemping to retrieve pilots profile.", exn.Message));
            }
            return null;
        }


        [Authorize]
        public PartialViewResult GetAPITable(string pilotName)
        {
            ViewBag.IsActiveUser = string.Compare(pilotName, User.Identity.Name, true) == 0;
            ViewBag.PilotName = pilotName;            
            APITableViewModel viewModel = new APITableViewModel()
            {
                existingAPIKeys = _apiRepository.GetAllApisForPilot(pilotName),
                newAPIKey = new ApiKey()
            };
            return PartialView("Partials/_ApiTable", viewModel);
           // return PartialView("~/Views/Shared/Partials/_ApiTable.cshtml", new Dictionary<IEnumerable<ApiKey>, bool>() { { _apiRepository.GetAllApisForPilot(pilotName) , true} });
        }

        [Authorize]
        public PartialViewResult GetAddAPIForm()
        {
            return PartialView("Partials/_AddAPI", new ApiKey());
        }

        public PartialViewResult AddAPIKey(ApiKey newAPIKey)
        {
            if (ModelState.IsValid)
            {
                int resultVal = 0;
                if (newAPIKey.PilotName == null)
                    newAPIKey.PilotName = User.Identity.Name;

                _apiRepository.AddAPIKey(newAPIKey, out resultVal);
                return GetAPITable(newAPIKey.PilotName ?? User.Identity.Name);
            }
            else
                return GetAPITable(newAPIKey.PilotName ?? User.Identity.Name);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
