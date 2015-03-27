using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoppsWebPlatform_Revamp.Models;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using HoppsWebPlatform_Revamp.DataAccess;
using HoppsWebPlatform_Revamp.Utilities;
using System.Web.Security;
using System.Xml;

namespace HoppsWebPlatform_Revamp.Controllers
{
    public class RecruitmentController : Controller
    {

        #region Properties

        private NLog.Logger _logger;
        private IRecruitmentRepository _recruitmentRepository;
        private IApiRepository _apiRepository;
        private IAltRepository _altRepository;
        private IEveDBRepository _eveDbRepository;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public RecruitmentController(IRecruitmentRepository recruitmentRepository, IApiRepository apiRepository, IAltRepository altRepository, IEveDBRepository iEveDBRepository)
        {
            _recruitmentRepository = recruitmentRepository;
            _apiRepository = apiRepository;
            _altRepository = altRepository;
            _eveDbRepository = iEveDBRepository;
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        /* **************** Application Related Methods ************* */
        #region P1 Related Methods

        /// <summary>
        /// Main recruitment screen showing admins the existing applications, and what to do to join etc..
        /// </summary>
        /// <returns>Main recruitment view </returns>
        [Authorize(Roles = "Guest, Director, Recruiter_Jr, Recruiter_Sr")]
        public ActionResult Index()
        {
            IEnumerable<RecruitmentApplication> personalApplications = _recruitmentRepository.GetRecruitApplicationsByName(User.Identity.Name);
            bool activeApp = personalApplications.Any(x => x.Active);

            //If there is an active app, give the recruit a message about the status
            if (activeApp)
            {
                string statusMessage = "";
                switch (personalApplications.First(x => x.Active).Status)
                {
                    case "Rejected":
                        statusMessage = "Unfortunatly your application to The Drunken Empire has been rejected";
                        break;
                    case "Accepted":
                        statusMessage = "Your application to The Drunken Empire has been accepted. Please get in contact with Natalie Cruella or TheCenobite to continue with joining.";
                        break;
                    default:
                        statusMessage = "Your application is still in process, please wait or contact a recruiter in our public channel <button onclick\"CCPEVE.joinChannel('HOPPS BREWERY');\">HOPPS BREWERY</button>";
                        break;
                }
                ViewBag.StatusMessage = statusMessage;
            }

            IEnumerable<RecruitmentApplication> applications = _recruitmentRepository.GetActiveApplications();
            return View(applications);
        }

        /// <summary>
        /// Recruit application to corporation
        /// </summary>
        /// <returns>Application to corporation view</returns>
        [Authorize]
        public ActionResult Apply(string userName)
        {
            bool personalCreation = (userName == null);
            if (userName == null) userName = User.Identity.Name;
            IEnumerable<RecruitmentApplication> applications = null;
            try
            {
                applications = _recruitmentRepository.GetRecruitApplicationsByName(userName);
            }
            catch (Exception exn)
            {
                return View("~/CustomError.cshtml", "An error occured whilst checking if you have an application. Please try again or contact an Admin/Director");
            }

            bool activeApp = applications.Any(x => x.Active);

            //If there is an active app, give the recruit a message about the status
            if (activeApp)
            {
                string statusMessage = "";
                switch (applications.First(x => x.Active).Status)
                {
                    case "Rejected":
                        statusMessage = "Unfortunatly " + userName + "'s application to The Drunken Empire has been rejected";
                        break;
                    case "Accepted":
                        statusMessage = userName + "'s application to The Drunken Empire has been accepted. Please get in contact with Natalie Cruella or TheCenobite to continue with joining.";
                        break;
                    default:
                        statusMessage = userName + "'s application is still in process, please wait or contact a recruiter in our public channel <button onclick\"CCPEVE.joinChannel('HOPPS BREWERY');\">HOPPS BREWERY</button>";
                        break;
                }
                ViewBag.StatusMessage = statusMessage;
                //Returns the Active App view with the appropriate message
                return View("AllreadyActiveApp");
            }

            RecruitmentApplication baseApplication = new RecruitmentApplication()
            {
                Questions = _recruitmentRepository.GetAllActiveQuestions(),
                ApplicantName = (personalCreation) ? null : userName,
                ApiKeys = _apiRepository.GetAllApisForPilot(userName)
            };

            return View(baseApplication);
        }

        /// <summary>
        /// Post method for Application to corporation view
        /// </summary>
        /// <param name="app">Posted recruit application</param>
        /// <returns>Completion view or Application view highlighting errors.</returns>
        [HttpPost]
        public ActionResult Apply(RecruitmentApplication app)
        {
            if (app.ApplicantName != null && (User.Identity.Name.ToUpper() != app.ApplicantName.ToUpper()))
            {
                if (!CustomHTMLHelpers.IsUserInAnyRole(User.Identity.Name, "Director,RecruiterSenior,RecruiterJunior"))
                {
                    ModelState.AddModelError("", "Applicant name missing or you are not authorised to perform applications");
                }
            }

            //If the applicant name is null, then its a self application. Set the name as the current user
            if (app.ApplicantName == null)
                app.ApplicantName = User.Identity.Name;
            else
            {
                //The application is created on behalf of someone, so make sure the user is a P2 recruiter etc.. so that someone cant immitate another user (Form manipulation)
                if (!Utilities.CustomHTMLHelpers.IsUserInAnyRole(User.Identity.Name, "Director, RecruiterJunior, RecruiterSenior"))
                {
                    ViewBag.ApplicantName = app.ApplicantName;
                    _logger.Warn(string.Format("User attempted to create an Application for: {0}", app.ApplicantName));
                    return View("UnauthorisedToApply");
                }
            }

            //No matter what, the P1 is the current user
            app.P1Recruiter = User.Identity.Name;

            if (app.ApiKeys.Count() == 0)
            {
                ModelState.AddModelError("", "An API key is required.");
            }

            int position = 0;
            foreach (ApiKey key in app.ApiKeys)
            {
                //If the API's vCode and KeyID is default, remove it from the list and any associated modelstate errors.
                if (key.VCode == null && key.KeyID == 0)
                {
                    List<ApiKey> keys = app.ApiKeys.ToList();
                    keys.RemoveAt(position);
                    app.ApiKeys = keys;
                    ModelState["APIKeys[" + position + "].KeyID"].Errors.Clear();
                    ModelState["APIKeys[" + position + "].VCode"].Errors.Clear();
                    ModelState.Remove("APIKeys[" + position + "].KeyID");
                    ModelState.Remove("APIKeys[" + position + "].VCode");
                }
                position++;
            }
            position = 0;
            try
            {
                //Process the api key (Add api, and alts)
                foreach (ApiKey key in app.ApiKeys)
                {
                    key.PilotName = app.ApplicantName;
                    AddAPIAfterValidation(key, position);
                    position++;
                }
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("Failed to add API for recruit application - Exception : {0}", exn.Message));
                return View("~/CustomError.cshtml", "API Failed to be added to your application. Please contact a Director or try again.");
            }

            //If the whole model is valid, proceed
            if (ModelState.IsValid)
            {
                //If there are any notes, add them to the field as a quote style
                string notes = app.Notes;
                if (notes != null && notes.Trim().Length > 0)
                {
                    app.Notes = app.ApplicantName + " Says <i>\"" + notes + "\"</i><hr />";
                }

                //Create the application 
                long appID = 0;
                bool success = false;
                try
                {
                    success = CreateApplication(app, out appID);
                    _logger.Info(string.Format("User applied to the corporation"));
                }
                catch (Exception exn)
                {
                    _logger.Error(string.Format("User failed to create a recruit application - Exception : {0}", exn.Message));
                    return View("~/CustomError.cshtml", "Failed to create your recruit application. Please contact a Director or try again.");
                }
                bool apiSuccess = true;
                //If everything was a success, show the appropriate screens
                return (success && apiSuccess) ? RedirectToAction("ApplicationConfirmation", new { appID = appID }) : RedirectToAction("ApplicationFailed");
            }
            else
            {
                //Get list of questions
                IEnumerable<RecruitmentApplicationQuestion> questions = _recruitmentRepository.GetAllActiveQuestions();
                //Update the recruit applications questions with the corresponding description to allow re-submission of form
                app.Questions.ToList().ForEach(x => x.Description = questions.Where(y => y.ID == x.ID).First().Description);

                return View(app);
            }
        }

        /// <summary>
        /// Does basic API checks on an application api key and adds it to the Database 
        /// </summary>
        /// <param name="newAPI">New API for application to add</param>
        /// <param name="position">Position which the API is found in the list so any API errors can be set based on ID position value </param>
        /// <returns>Switch based on whether the API was successfully added</returns>
        private bool AddAPIAfterValidation(ApiKey newAPI, int position)
        {
            bool validEntry = true;
            try
            {
                if (!APIHelper.EVE_IsAPIValid(newAPI.KeyID, newAPI.VCode))
                {
                    validEntry = false;
                    ModelState.AddModelError("apiKeys[" + position + "].KeyID", "API Key is not valid");
                }
                else
                {
                    if (!APIHelper.EVE_IsAPIAccountWide(newAPI.KeyID, newAPI.VCode))
                    {
                        validEntry = false;
                        ModelState.AddModelError("apiKeys[" + position + "].KeyID", "API key not set to account wide.");
                    }
                    if (!APIHelper.EVE_IsAPIKeyFullAccountPermission(newAPI.KeyID, newAPI.VCode))
                    {
                        validEntry = false;
                        ModelState.AddModelError("apiKeys[" + position + "].KeyID", "API Key not set to full access. Access Mask: 268435455");
                    }
                    if (!APIHelper.EVE_IsAPISetToNotExpire(newAPI.KeyID, newAPI.VCode))
                    {
                        validEntry = false;
                        ModelState.AddModelError("apiKeys[" + position + "].KeyID", "API Key not set to never expire.");
                    }
                }
                if (validEntry)
                {
                    int addResult = 1;
                    foreach (string character in APIHelper.EVE_GetCharactersOnAPI(newAPI.KeyID, newAPI.VCode).Select(x => x.Key))
                    {
                        if (character != newAPI.PilotName)
                            _altRepository.AddAlt(new Alt() { MainName = newAPI.PilotName, AltName = character, AltPurpose = "Unknown" });

                        _apiRepository.AddAPIKey(new ApiKey() { PilotName = character, KeyID = newAPI.KeyID, VCode = newAPI.VCode, Valid = true }, out addResult);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst trying to add an API for recruit application. Exception: {0}", exn.Message));
                ModelState.AddModelError("apikeys[" + position + "].KeyID", "Error occured whilst checking API. Please try again, or contact an admin");
                return false;
            }
        }

        /// <summary>
        /// Creates a whole application, including the questions and base app.
        /// </summary>
        /// <param name="app">Application to create</param>
        /// <returns>Switch based on whether the applicaton was created successfully.</returns>
        [Authorize]
        private bool CreateApplication(RecruitmentApplication app, out long appID)
        {
            appID = 0;
            try
            {
                //Create the application
                bool created = _recruitmentRepository.CreateApplication(app, out appID);

                //If the application failed to create, return false
                if (!created)
                {
                    _logger.Error(string.Format("Recruit Application for : {0} failed to be created", User.Identity.Name));
                    return false;
                }
                _logger.Info(string.Format("Recruit Application for : {0} was successfully created", User.Identity.Name));
                //Add the question answers
                foreach (RecruitmentApplicationQuestion question in app.Questions)
                {
                    _recruitmentRepository.AddQuestionForApplication(appID, question.ID, question.Answer);
                }
                return true;
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst attempting to create a recruit application for : {0}. Exception: {1}", app.ApplicantName, exn.Message));
                return false;
            }
        }

        /// <summary>
        /// Returns the application successfully created view
        /// </summary>
        /// <returns>Application successfully added view</returns>
        [Authorize]
        public ActionResult ApplicationConfirmation(long appID)
        {
            try
            {
                ViewBag.ApplicationURL = Request.Url.ToString().Replace(Request.RawUrl, "/Recruitment/ViewApplication/" + appID);
                ViewBag.AppID = appID;
                ViewBag.PilotID = Utilities.APIHelper.EVE_GetPilotIDByName(User.Identity.Name);
                ViewBag.ApplicantName = _recruitmentRepository.GetRecruitApplicationByID(appID).ApplicantName;
                return View("ApplicationConfirmation");
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst attempting to display application confirmation for application : {0}. Exception: {1}", appID, exn.Message));
                return View("~/CustomError.cshtml", "An error occured whilst displaying your confirmation. However your application was successfully submitted.");
            }

        }

        #endregion

        /* ************ Security Check Related Methods ************ */

        #region Security Check Related Methods

        /// <summary>
        /// Gets the base recruitment application view
        /// </summary>
        /// <param name="application">Application to view</param>
        /// <returns>Recruitment application view</returns>
        public ActionResult GetApplicationView(RecruitmentApplication application)
        {
            return View("Partials/_Application", application);
        }

        /// <summary>
        /// Gets the recruit application view by application ID
        /// </summary>
        /// <param name="appID">Application ID to view</param>
        /// <returns>Recruit application view</returns>
        [Authorize(Roles = "Director, RecruiterSenior")]
        [HttpGet]
        public ActionResult ViewApplication(long id)
        {
            try
            {
                RecruitmentApplication application = _recruitmentRepository.GetRecruitApplicationByID(id);
                return View(application);
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst displaying the View Application page for application : {0}. Exception: {1}", id, exn.Message));
                return View("~/CustomError.cshtml", "An error occured whilst attempting to create application view page. Please try again or contact an admin");
            }
        }

        /// <summary>
        /// Gets the P2 screen for the application
        /// </summary>
        /// <param name="appID">App ID to get recruit application for</param>
        /// <returns>P2 View for application</returns>
        [HttpGet]
        //[OutputCache(Duration = 300, Location = System.Web.UI.OutputCacheLocation.ServerAndClient)]
        [Authorize(Roles = "Director, RecruiterSenior")]
        public ActionResult BackgroundCheck(long appID)
        {
            ApplicationBackgroundCheckViewModel viewModel = new ApplicationBackgroundCheckViewModel();
            try
            {
                RecruitmentApplication application = _recruitmentRepository.GetRecruitApplicationByID(appID);
                application.ApiKeys = _apiRepository.GetAllApisForPilot(application.ApplicantName);
                IEnumerable<string> pilots = application.ApiKeys.Select(x => x.PilotName);

                //This is the assignment of all the Security Check Modules. (This can be removed, and rendered through an AJAX request, as they are purely HTMLString markup)
                viewModel.Application = application;

                return View(viewModel);
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst creating background check view for application: {0}. Exception: {1}", appID, exn.Message));
                return View("~/CustomError.cshtml", "An error occured whilst attempting to create background check page. Please try again or contact an admin");
            }
        }

        /// <summary>
        /// Gets view for the CFC Blacklisted section
        /// </summary>
        /// <param name="characters">List of apis to get characters for the application</param>
        /// <returns>View for the CFC blacklist section</returns>
        public PartialViewResult GetCFCBlackListCheck(IEnumerable<ApiKey> apiKeys)
        {
            Dictionary<string, bool> blackListed = new Dictionary<string, bool>();
            foreach (ApiKey key in apiKeys)
            {
                foreach (string pilot in APIHelper.EVE_GetCharactersOnAPI(key.KeyID, key.VCode).Select(x => x.Key))
                {
                    try
                    {
                        bool listed = APIHelper.RC_IsPilotBlacklisted(pilot);
                        if (!blackListed.Contains(new KeyValuePair<string, bool>(pilot, listed)))
                        {
                            blackListed.Add(pilot, listed);
                        }
                    }
                    catch (Exception exn)
                    {
                        _logger.Error(string.Format("An error occured whilst attempting to get black list check on: {0}. Exception : {1}", pilot, exn.Message));
                        ViewBag.Header = "CFC Blacklist Check";
                        ViewBag.Message = "Unable to retrieve blacklist data from CFC. Please check manually!";
                        return PartialView("Partials/_BackgroundCheckFailed");
                    }
                }
            }
            return PartialView("Partials/_CFCBlackListCheck", blackListed);
        }

        /// <summary>
        /// Gets view for the Wallet Journal Section
        /// </summary>
        /// <param name="apiKeys">List of API Keys to check against</param>
        /// <returns>Viewfor the Wallet Journal section</returns>
        public PartialViewResult GetWalletTransferCheck(IEnumerable<ApiKey> apiKeys)
        {
            List<WalletJournalEntry> entries = new List<WalletJournalEntry>();

            try
            {
                foreach (ApiKey api in apiKeys)
                {
                    entries.AddRange(GetWalletTransferFlags(api));
                }
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst attempting to retrieve wallet transfer markup. Exception : {0}", exn.Message));
                ViewBag.Header = "Wallet Transaction Check";
                ViewBag.Message = "Unable to process wallet transactions. Please check manually!";
                return PartialView("Partials/_BackgroundCheckFailed");
            }
            return PartialView("Partials/_WalletJournalCheck", entries);
        }

        /// <summary>
        /// Gets any wallet transfer flags based on config min amount, per account
        /// </summary>
        /// <param name="api">API to check</param>
        /// <returns>List of any wallet transaction flags</returns>
        public List<WalletJournalEntry> GetWalletTransferFlags(ApiKey api)
        {
            List<WalletJournalEntry> flags = new List<WalletJournalEntry>();
            //If the API is readable, read it.
            if (APIHelper.EVE_IsAPIFullConsistantAccount(api.KeyID, api.VCode))
            {
                try
                {
                    //Get any of the wallet journal "Flags" 
                    List<WalletJournalEntry> apiEntries = APIHelper.EVE_GetWalletTransactionsBasedOnTypeAndMin(api.KeyID, api.VCode, APIHelper.EVE_GetPilotIDByName(api.PilotName), 10, Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["RecruitAppMinimumWalletDonation"].ToString())).ToList();
                    //Add any wallet journal flags to a collection of entries
                    foreach (WalletJournalEntry apiEntry in apiEntries)
                        flags.Add(apiEntry);
                }
                catch (Exception exn)
                {
                    _logger.Error(string.Format("An error occured whilst attempting to retrieve wallet entries for api KeyID: {0}, vCode : {1}. - Exception : {2}", api.KeyID, api.VCode, exn.Message));
                    flags.Add(new WalletJournalEntry() { Sender = api.PilotName, Description = "An error occured whilst querying the API" });
                }
            }
            else
            {
                flags.Add(new WalletJournalEntry() { Description = "Invalid API", Sender = api.PilotName });
            }
            return flags;
        }

        /// <summary>
        /// Gets a partial view containing the Mail Message check section
        /// </summary>
        /// <param name="apiKeys">List of API Keys to check against</param>
        /// <returns>View for the flagged mail messages section</returns>
        public PartialViewResult GetMailMessageCheck(IEnumerable<ApiKey> apiKeys)
        {
            List<MailMessage> flaggedEveMails = new List<MailMessage>();
            try
            {
                foreach (ApiKey api in apiKeys)
                {
                    bool isAPIGood = false;
                    try
                    {
                        //Checks is the API is valid for Eve Mail retrieval
                        isAPIGood = APIHelper.EVE_IsAPIKeyFullAccountPermission(api.KeyID, api.VCode);
                    }
                    catch (Exception)
                    {
                        //If there is an exception, we put it in a eve mail to display
                        flaggedEveMails.Add(new MailMessage() { Body = "Unable to retrieve data from the API server for " + api.PilotName, Subject = "Unable to retrieve API data" });
                        continue;
                    }
                    if (isAPIGood)
                        //Add all the evemails that get flagged
                        flaggedEveMails.AddRange(GetMailMessageFlagsForCharacter(api));
                    else
                        //API was invalid, like an exception, put it in an eve mail to display
                        flaggedEveMails.Add(new MailMessage() { Body = " Invalid API for" + api.PilotName, Subject = "Invalid API for" + api.PilotName });
                }
                return PartialView("Partials/_EveMailCheck", flaggedEveMails);
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst attempting to retrieve background check email markup. Exception : {0}", exn.Message));
                ViewBag.Header = "EVE Mail Check";
                ViewBag.Message = "Unable to process EVE Mails. Please check manually!";
                return PartialView("Partials/_BackgroundCheckFailed");
            }
        }

        /// <summary>
        /// Gets any mail messages that are flagged based on content
        /// </summary>
        /// <param name="api">API to check</param>
        /// <returns>List of any mail message flags</returns>
        public List<MailMessage> GetMailMessageFlagsForCharacter(ApiKey api)
        {
            List<MailMessage> messages = new List<MailMessage>();
            List<MailMessage> flaggedMessages = new List<MailMessage>();

            messages = APIHelper.EVE_GetMailMessagesForCharacter(api.KeyID, api.VCode, APIHelper.EVE_GetPilotIDByName(api.PilotName)).ToList();
            IEnumerable<String> keyWords = _recruitmentRepository.GetBackgroundCheckMailKeywords();

            foreach (MailMessage mail in messages)
            {
                foreach (string keyword in keyWords)
                {
                    if (mail.Body.Split(' ').Any(x => x.ToUpper() == keyword.ToUpper()))
                    {
                        flaggedMessages.Add(mail);
                        break;
                    }
                }
            }
            return flaggedMessages;
        }

        /// <summary>
        /// Does the background check result
        /// </summary>
        /// <param name="applicationID">Application ID</param>
        /// <param name="notes">Any notes</param>
        /// <param name="action">result of background check - Index (1 based)</param>
        /// <returns>Redirect request to the background check view after changes</returns>
        public ActionResult FinalizeBackgroundCheck(int applicationID, string notes, string action)
        {
            RecruitmentApplication application = _recruitmentRepository.GetRecruitApplicationByID(applicationID);

            if (application != null && application.Active == true)
            {
                string actionText = "";
                switch (action)
                {
                    case "1":
                        actionText = "Accepted";
                        break;
                    case "2":
                        actionText = "Declined";
                        break;
                    case "3":
                        actionText = "Escalated";
                        break;
                }
                string wholeNotes = User.Identity.Name + " Says \"<i>" + notes + " \"</i><hr />" + application.Notes;
                try
                {
                    _recruitmentRepository.UpdateApplicationBackgroundCheck(applicationID, wholeNotes, actionText, User.Identity.Name);
                    _logger.Info(string.Format("Background check for recruit application : {0} was completed with result : {1}", applicationID, action));
                }
                catch (Exception exn)
                {
                    _logger.Error(string.Format("An error occured whilst attempting to complete the background check for application : {0}. Exception : {1}", applicationID, exn.Message));
                    return View("~/CustomError.cshtml", string.Format("Unable to process the background check for : {0}. Please try again or contact an admin", applicationID));
                }

            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Gets a serialized message for the recruiter to send as confirmation
        /// </summary>
        /// <param name="appID">Application ID</param>
        /// <returns>Serialized message</returns>
        public JsonResult GetConfirmationEVEMail(long appID)
        {
            RecruitmentApplication app = _recruitmentRepository.GetRecruitApplicationByID(appID);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Sender", User.Identity.Name);
            data.Add("Reciever", app.ApplicantName);

            string body = EmailHelper.ConvertViewToString(this, "Partials/ConfirmationEVEMail", new ViewDataDictionary() { Model = data });
            MailMessage message = new MailMessage() { Body = body, Subject = "Application To HOPPS Has Been Approved", ToPilotIdList = new List<long>() { APIHelper.EVE_GetPilotIDByName(app.ApplicantName) } };
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets a serialized message for the recruiter to send as rejection
        /// </summary>
        /// <param name="appID">Application ID</param>
        /// <returns>Serialized message</returns>
        public JsonResult GetRejectionEVEMail(long appID)
        {
            RecruitmentApplication app = _recruitmentRepository.GetRecruitApplicationByID(appID);
            Dictionary<string, string> data = new Dictionary<string, string>();
            data.Add("Sender", User.Identity.Name);
            data.Add("Reciever", app.ApplicantName);

            string body = EmailHelper.ConvertViewToString(this, "Partials/RejectionEVEMail", new ViewDataDictionary() { Model = data });
            MailMessage message = new MailMessage() { Body = body, Subject = "Application To HOPPS Has Been Rejected", ToPilotIdList = new List<long>() { APIHelper.EVE_GetPilotIDByName(app.ApplicantName) } };
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Completes the recruit application with either an accepted or declined result
        /// </summary>
        /// <param name="result">Action based on whether the application was accepted to declined</param>
        /// <param name="appID">Application ID</param>
        /// <returns>Redirect to index</returns>
        public ActionResult CompleteApplication(string result, long appID)
        {
            if (User.IsInRole("Director"))
            {
                try
                {
                    _recruitmentRepository.CompleteApplication(appID, result, User.Identity.Name);
                    _logger.Info(string.Format("Recruit application : {0} was completed with result {1}", appID, result));
                }
                catch (Exception exn)
                {
                    _logger.Error(string.Format("An error occured whilst attempting to complete recruit application : {0}. Exception : {1}", appID, exn.Message));
                    return View("~/CustomError.cshtml", string.Format("Unable to complete the application : {0}. Please try again or contact an admin", appID));
                }
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Gets view for the Ship fitting section
        /// </summary>
        /// <param name="apiKeys">API Keys to check fittings againt</param>
        /// <returns>View for ship fitting section</returns>
        public PartialViewResult GetShipFittingCheck(IEnumerable<ApiKey> apiKeys)
        {
            try
            {
                //Gets all of the ship fittings to check
                IEnumerable<RecruitApplicationShipFitting> shipFittings = _recruitmentRepository.GetShipFittingsByActivity(true);
                bool apiGood = true;

                // The complete list to use for later rendering
                List<ShipFittingCheckResult> completeFittingResults = new List<ShipFittingCheckResult>();

                //A ship fitting container for errors
                ShipFittingCheckResult errorResult = new ShipFittingCheckResult();

                foreach (ApiKey key in apiKeys)
                {
                    long characterID = APIHelper.EVE_GetPilotIDByName(key.PilotName);

                    errorResult.PilotName = key.PilotName;
                    try
                    {
                        //Checks if the API is usable
                        apiGood = APIHelper.EVE_IsAPIFullConsistantAccount(key.KeyID, key.VCode);
                    }
                    catch (Exception exn)
                    {
                        errorResult.ErrorMessage = key.PilotName + " - Unable to retrieve data from the API server";
                        completeFittingResults.Add(errorResult);
                        continue;
                    }
                    if (!apiGood)
                    {
                        errorResult.ErrorMessage = key.PilotName + " - Invalid API for pilot";
                        completeFittingResults.Add(errorResult);
                        continue;
                    }

                    //Gets a list of all the characters skills
                    Dictionary<int, int> skills = APIHelper.EVE_GetCharactersSkills(key.KeyID, key.VCode, characterID);

                    foreach (RecruitApplicationShipFitting fitting in shipFittings)
                    {

                        ShipFittingCheckResult fittingResult = new ShipFittingCheckResult();
                        fittingResult.PilotName = key.PilotName;
                        fittingResult.ShipFitting = fitting;

                        List<SkillPrerequisite> missingSkills = new List<SkillPrerequisite>();

                        //Gets all the required skills for the fittings ship
                        IEnumerable<SkillPrerequisite> shipSkills = _eveDbRepository.GetItemsPrimaryRequirements(null, fitting.ShipType);
                        //Check if the pilot has all the skills required, if not, the missing skills are out'd
                        bool meetItemPreReqs = DoesPilotHaveAllPrereqs(skills, shipSkills, out missingSkills);
                        fittingResult.MissingShipSkills = missingSkills;

                        //Gets all the required skills for the fitting modules / hardware
                        IEnumerable<SkillPrerequisite> moduleSkills = GetFittingPrerequisitesExcludingShipById(fitting.ID);
                        missingSkills.Clear();
                        //Check if the pilot has all the skills required, if not, the missing skills are out'd
                        meetItemPreReqs = DoesPilotHaveAllPrereqs(skills, moduleSkills, out missingSkills);
                        fittingResult.MissingModuleSkills = missingSkills;

                        completeFittingResults.Add(fittingResult);
                    }
                }
                return PartialView("Partials/_ShipFittingMarkup", completeFittingResults);
            }
            catch (Exception)
            {
                ViewBag.Header = "Ship Fitting Check";
                ViewBag.Message = "Unable to process ship fittings. Please check manually!";
                return PartialView("Partials/_BackgroundCheckFailed");
            }

        }

        /// <summary>
        /// Checks if a character has all of the listed required skills
        /// </summary>
        /// <param name="charSkills">Dictionary of skills of the character</param>
        /// <param name="requiredSkills">List of skills that are to be checked for.</param>
        /// <param name="missingSkills">Out list of missing skills.</param>
        /// <returns>Switch based on whether all of the skills are met</returns>
        private bool DoesPilotHaveAllPrereqs(Dictionary<int, int> charSkills, IEnumerable<SkillPrerequisite> requiredSkills, out List<SkillPrerequisite> missingSkills)
        {
            missingSkills = new List<SkillPrerequisite>();

            foreach (SkillPrerequisite preReqs in requiredSkills)
            {
                //Create a temp skillPrerequisite that will be used for missing (Required level filled in later)
                SkillPrerequisite newMissingSkill = new SkillPrerequisite() { RequiredLevel = 0, SkillName = preReqs.SkillName, SkillID = preReqs.SkillID, CurrentLevel = 0 };
                //Is the current preReq skill present in the characters skill collection, regardless of level.
                if (charSkills.Any(x => x.Key == preReqs.SkillID))
                {
                    //Retrieve the pilots skill from the dictionary
                    KeyValuePair<int, int> curSkill = charSkills.Where(x => x.Key == preReqs.SkillID).First();
                    //Check if the pilots skill level is less than what is required.
                    if (curSkill.Value < preReqs.RequiredLevel)
                    {
                        //Set the required skill level to the users current level, Doesnt make grammatical sense, but its neat to pass back the current level.
                        newMissingSkill.RequiredLevel = preReqs.RequiredLevel;
                        newMissingSkill.CurrentLevel = curSkill.Value;
                        //Add the missingSkill to the collection
                        missingSkills.Add(newMissingSkill);
                    }
                }
                else
                {
                    //If the skill isnt in the characters skill collection at all, they are bad and it should be noted.!
                    newMissingSkill.RequiredLevel = preReqs.RequiredLevel;
                    missingSkills.Add(newMissingSkill);
                }
            }
            return missingSkills.Count == 0;
        }

        /// <summary>
        /// Gets a list of unique skills containing the max required skill level for a skill fitting based on its ID - (Excluding ship pre-reqs)
        /// </summary>
        /// <param name="fittingID">Id of fitting to get skills for.</param>
        /// <returns>Unique list of skills containing max required level</returns>
        public IEnumerable<SkillPrerequisite> GetFittingPrerequisitesExcludingShipById(int fittingID)
        {
            List<SkillPrerequisite> skills = new List<SkillPrerequisite>();

            //Get the XML data for the uploaded fitting and load it.
            string xmlData = _recruitmentRepository.GetShipFittingByID(fittingID).XMLData;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlData);

            //Hardwares contains all the module elements to get skills for
            XmlNodeList hardwares = doc.GetElementsByTagName("hardware");

            //List of module names on ship
            List<string> shipItems = new List<string>();

            foreach (XmlNode node in hardwares)
                shipItems.Add(node.Attributes["type"].Value);

            //Add all the skill requirements for each module into the skills list
            foreach (string item in shipItems.Distinct())
                skills.AddRange(_eveDbRepository.GetItemsPrimaryRequirements(null, item));

            //List of all the skills filtered, and updated to contain the max required skill level.
            List<SkillPrerequisite> uniqueSkills = new List<SkillPrerequisite>();

            //Filter the list of skills, and update the max required skill is neccessary.
            foreach (SkillPrerequisite skill in skills)
            {
                //If the skill is in the unique list, see if it needs updating
                if (uniqueSkills.Any(x => x.SkillID == skill.SkillID))
                {
                    SkillPrerequisite tempPre = uniqueSkills.Where(x => x.SkillID == skill.SkillID).First();

                    //Check if the current level is less than the new modules required level. 
                    if (tempPre.RequiredLevel < skill.RequiredLevel)
                        tempPre.RequiredLevel = skill.RequiredLevel;
                }
                else
                    //Skill is not in the unique list, so er .. add it. =)
                    uniqueSkills.Add(skill);
            }
            return uniqueSkills;
        }

        #endregion

    }
}
