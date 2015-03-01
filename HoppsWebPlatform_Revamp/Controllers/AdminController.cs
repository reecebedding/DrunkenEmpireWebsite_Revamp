using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoppsWebPlatform_Revamp.Models;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using HoppsWebPlatform_Revamp.DataAccess;
using HoppsWebPlatform_Revamp.Utilities;
using Newtonsoft.Json;
using System.Xml;
using System.Text;

namespace HoppsWebPlatform_Revamp.Controllers
{
    [Attributes.Authorize(Roles="Director")]
    public class AdminController : Controller
    {
    
    #region Properties

        private IAppSettingsRepository _appSettingsRepository;
        private IRecruitmentRepository _recruitmentRepository;
        private ILogRepository _logRepository;
        private NLog.Logger _logger;

    #endregion


        public AdminController(IAppSettingsRepository appSettingsRepository, IRecruitmentRepository recruitmentRepository, ILogRepository logRepository)
        {
            _appSettingsRepository = appSettingsRepository;
            _recruitmentRepository = recruitmentRepository;
            _logRepository = logRepository;
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }
       

        /// <summary>
        /// Main entry point for administration, contains links to specific admin pages.
        /// </summary>
        /// <returns>View of links</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Gets the application configuration view 
        /// </summary>
        /// <returns>Application configuration view</returns>
        public ActionResult Configuration()
        {
            ConfigurationViewModel newViewModel = new ConfigurationViewModel()
            {
                DirectorAPIKey = _appSettingsRepository.GetDirectorAPI(),
                CorporationAPIKey = _appSettingsRepository.GetCorporationAPI()
            };
            return View(newViewModel);
        }

        /// <summary>
        /// Saves the application configuration settings
        /// </summary>
        /// <param name="view">Settings to be saved for configuration</param>
        /// <returns>TBD</returns>
        [HttpPost]
        public ActionResult SaveConfiguration(ConfigurationViewModel view)
        {
            List<string> successMessages = new List<string>();
            if (ModelState.IsValid)
            {
                //Module - Processing new Director API key.
                #region ProcessDirectorAPI
                if (VerifyDirectorAPI(view.DirectorAPIKey))
                {
                    int directorUpdateStatus;
                    _appSettingsRepository.UpdateDirectorAPIKey(view.DirectorAPIKey, out directorUpdateStatus);
                    if (directorUpdateStatus == 1)
                            successMessages.Add("Director API successfully updated");
                        //Dont put any other Update status' as the api was only updated IF the result was 1. - 2 = Allready exists.
                    else if (directorUpdateStatus != 2)
                        ModelState.AddModelError("", "Error updating the director API Key, please try again or contact Natalie Cruella");
                }
                #endregion

                //Module - Processing new corporation API key.
                #region ProcessCorporationAPI

                if (VerifyCorporationAPI(view.CorporationAPIKey))
                {
                    int corporationUpdateStatus;
                    _appSettingsRepository.UpdateCorporationAPIKey(view.CorporationAPIKey, out corporationUpdateStatus);
                    if (corporationUpdateStatus == 1)                    
                            successMessages.Add("Corporation API successfully updated");   
                        //Dont put any other Update status' as the api was only updated IF the result was 1. - 2 == Allready exists.
                    else if (corporationUpdateStatus != 2)
                        ModelState.AddModelError("", "Error updating the corporation API key, please try again or contact Natalie Cruella");
                }
                
                
                #endregion
            }
            //Assign all the success messages to the viewdata object to be validated in view.
            ViewData[GlobalValues.ValidationValidMessage.ToString()] = successMessages;
            return View("Configuration", view);
        }


        /// <summary>
        /// Validates whether the Director API key is valid for all settings, and adds errors to ModelState if not.
        /// </summary>
        /// <param name="directorAPIKey">Director API key to check.</param>
        /// <returns>Switch based on whether API key is valid.</returns>
        private bool VerifyDirectorAPI(ApiKey directorAPIKey)
        {
            bool isAPIValid = true;
            //If the API is invalid, dont bother progressing the other checks as the key ... well .. isnt a key.
            if (!APIHelper.EVE_IsAPIValid(directorAPIKey.KeyID, directorAPIKey.VCode))
            {
                ModelState.AddModelError("", "Director API key is invalid");
                isAPIValid = false;
            }
            else
            {
                //Checks if the API is set to a character API. Dont progress as if its not a character API, the other checks are pointless.
                if (!APIHelper.EVE_IsAPICharacterWide(directorAPIKey.KeyID, directorAPIKey.VCode))
                {
                    ModelState.AddModelError("", "Director API key is not set to character for director");
                    isAPIValid = false;
                }
                else
                {
                    //If API is a character type, then check that the only character is in fact a director in the corp.
                    if (!APIHelper.EVE_IsPilotInRole(APIHelper.EVE_GetCharactersOnAPI(directorAPIKey.KeyID, directorAPIKey.VCode).First().Key, "roleDirector", directorAPIKey.KeyID, directorAPIKey.VCode))
                    {
                        ModelState.AddModelError("", "Character on the Director API is not a director");
                        isAPIValid = false;
                    }
                    else
                    {
                        //If the API is of character type and the pilot is a director, then simply check it is set to full and wont expire.
                        if (!APIHelper.EVE_IsAPIKeyFullAccountPermission(directorAPIKey.KeyID, directorAPIKey.VCode))
                        {
                            ModelState.AddModelError("", "Director API not set to full permissions");
                            isAPIValid = false;
                        }
                        if (!APIHelper.EVE_IsAPISetToNotExpire(directorAPIKey.KeyID, directorAPIKey.VCode))
                        {
                            ModelState.AddModelError("", "Director API not set to never expire.");
                            isAPIValid = false;
                        }
                    }
                }
            }
            return isAPIValid;
        }

        /// <summary>
        /// Validates whether the Director API key is valid for all settings, and adds errors to ModelState if not.
        /// </summary>
        /// <param name="directorAPIKey">Director API key to check.</param>
        /// <returns>Switch based on whether API key is valid.</returns>
        private bool VerifyCorporationAPI(ApiKey corporationAPIKey)
        {
            bool isAPIValid = true;
            //If the API is invalid, dont bother progressing the other checks as the key ... well .. isnt a key.
            if (!APIHelper.EVE_IsAPIValid(corporationAPIKey.KeyID, corporationAPIKey.VCode))
            {
                ModelState.AddModelError("", "Corporation API key is invalid");
                isAPIValid = false;
            }
            else
            {
                //Check that the API key is set to corporation type
                if (!APIHelper.EVE_IsAPICorporationWide(corporationAPIKey.KeyID, corporationAPIKey.VCode))
                {
                    ModelState.AddModelError("", "Corporation API key is not set to corporation type.");
                    isAPIValid = false;
                }
                else
                {
                    //Check that the API is set to full permissions
                    if (!APIHelper.EVE_IsAPIKeyFullCorporationPermission(corporationAPIKey.KeyID, corporationAPIKey.VCode))
                    {
                        ModelState.AddModelError("", "Corporation API key is not set to full permissions.");
                        isAPIValid = false;
                    }
                    else
                    {
                        //Check that the API is set to never expire.
                        if (!APIHelper.EVE_IsAPISetToNotExpire(corporationAPIKey.KeyID, corporationAPIKey.VCode))
                        {
                            ModelState.AddModelError("", "Corporation API key is not set to never expire.");
                            isAPIValid = false;
                        }
                    }
                }
            }
            return isAPIValid;
        }

        /// <summary>
        /// Gets the permission modification admin page.
        /// </summary>
        /// <returns>Permission modification admin view</returns>
        public ActionResult Permissions()
        {
            return View();
        }

        /// <summary>
        /// Main recruitment Administration screen
        /// </summary>
        /// <returns>Administration view for recruitment</returns>
        public ActionResult Recruitment()
        {
            RecruitmentViewModel viewModel = new RecruitmentViewModel()
            {
                KeyWords = _appSettingsRepository.GetSettingByName("BackgroundCheckEVEMailWords").Value     
            };
            return View(viewModel);
        }

        /// <summary>
        /// Gets a recruitmentApplication by id and serializes it to JSON.
        /// </summary>
        /// <param name="id">Id of recruit application</param>
        /// <returns>JSON syntax of recuitment application</returns>
        [HttpGet]
        public JsonResult GetQuestion(long id)
        {
            RecruitmentApplicationQuestion question = _recruitmentRepository.GetQuestionByQuestionID(id);
            return Json(question, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Edits and application and returns a JSON object containing a success value and any modelstate errors, serialized.
        /// </summary>
        /// <param name="question">Recruit application to update</param>
        /// <returns>Json result of success value and model state errors.</returns>
        [HttpPost]
        public JsonResult EditQuestion(RecruitmentApplicationQuestion question)
        {
            ModelState.Remove("Answer");

            if (ModelState.IsValid)
            {
                bool result = _recruitmentRepository.EditQuestion(question);
                if (result)
                    _logger.Info(string.Format("Question ID: {0} was successfully updated.", question.ID));
                else
                    _logger.Warn(string.Format("Question ID: {0} failed to update", question.ID));
                return Json(result);
            }
            else
            {
                //Returns serialized version of the ModelState errors in order to process the "Validation" summary JS.
                return CustomHTMLHelpers.ConvertModelStateErrorsToJson(ModelState);
            }
        }

        /// <summary>
        /// Edits the keywords for EVE mail background checks
        /// </summary>
        /// <param name="KeyWords">CSV of keywords to search for</param>
        /// <returns>Integer value representing the transaction result</returns>
        [HttpPost]
        public JsonResult EditKeywords(string KeyWords)
        {
            int result = 0;
            bool updateResult = _appSettingsRepository.UpdateSettingByName("BackgroundCheckEVEMailWords", KeyWords, out result);
            return Json(updateResult);
        }

        /// <summary>
        /// Returns the partial view containing all of the recruit application questions
        /// </summary>
        /// <returns>Partial view of all recruit questions</returns>
        public ActionResult GetQuestionListView()
        {
            return View("QuestionList", _recruitmentRepository.GetAllQuestions());
        }

        /// <summary>
        /// Returns the partial view containing all of the keywords for the eve mail background check
        /// </summary>
        /// <returns>Partial view of all key words</returns>
        public ActionResult GetKeyWordsListView()
        {
            return View("Partials/_KeyWordList", _appSettingsRepository.GetSettingByName("BackgroundCheckEVEMailWords").Value.Split(','));
        }

        /// <summary>
        /// Creates a new recruit application
        /// </summary>
        /// <param name="question">New recruit application</param>
        /// <returns>JSON result containing success value and any model state errors.</returns>
        [HttpPost]
        public JsonResult AddQuestion(RecruitmentApplicationQuestion question)
        {
            //Removes the Answer and ID errors as these are not going to be present in a new question
            ModelState.Remove("Answer");
            ModelState.Remove("ID");
            try
            {
                if (ModelState.IsValid)
                {
                    bool result = _recruitmentRepository.AddQuestion(question);
                    if (result)
                        _logger.Info(string.Format("Question : {0} was succesfully added", question.Description));
                    else
                        _logger.Warn(string.Format("Question : {0} failed to update", question.Description));
                    return Json(result);
                }
                else
                {
                    //Returns a serialized version of the ModelState errors in order to process the "Validation" summary JS
                    return CustomHTMLHelpers.ConvertModelStateErrorsToJson(ModelState);
                }
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst attempting to create a new application question : {0}", exn.Message));

                return Json(false);
            }
        }

        /// <summary>
        /// Gets the main administration page for the Fitting Manager
        /// </summary>
        /// <returns>Fitting manager view</returns>
        public ActionResult FittingManager()
        {
            IEnumerable<RecruitApplicationShipFitting> fittings = _recruitmentRepository.GetShipFittingsByActivity(false);
            return View(fittings);
        }

        /// <summary>
        /// Adds a new recruit application fitting from a XML file upload. - Based on EFT upload format.
        /// </summary>
        /// <param name="file">EFT XML file</param>
        /// <returns>Original view of fitting manager after the creation of the new fitting</returns>
        [HttpPost]
        public ActionResult UploadFittingFile(HttpPostedFileBase file)
        {
            XmlDocument doc = new XmlDocument();
            RecruitApplicationShipFitting fitting = new RecruitApplicationShipFitting();

            try
            {
                //Takes the fittings XML binary data and adds it to a byte[] that is transformed into string (xmlData) format
                System.IO.BinaryReader br = new System.IO.BinaryReader(file.InputStream);
                byte[] binaryData = br.ReadBytes(Convert.ToInt32(file.InputStream.Length));
                string xmlData = Encoding.UTF8.GetString(binaryData);

                return UploadNewFittingFromXML(xmlData);                
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst trying to upload a new ship fitting. Exn: {0}", exn.Message));
                return View("~/Views/Shared/CustomError.cshtml", null, "An error occured with processing your file as it may be malformed. Please try again.");
            }
        }

        /// <summary>
        /// Adds a new recruit application fitting from XML data uploaded as a string
        /// </summary>
        /// <param name="fittingText">EFT XML data</param>
        /// <returns>Original view of fitting manager after the creation of the new fitting</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UploadFittingText(string fittingText)
        {
            try
            {
                return UploadNewFittingFromXML(fittingText);
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst trying to upload a new ship fitting. Exn: {0}", exn.Message));
                return View("~/Views/Shared/CustomError.cshtml", null, "An error occured with processing your file as it may be malformed. Please try again.");
            }
        }

        /// <summary>
        /// Method to create the new fitting in the database
        /// </summary>
        /// <param name="XML">XML data to create the recruit application from</param>
        /// <returns>Redirect to the original fitting manager page</returns>
        private ActionResult UploadNewFittingFromXML(string XML)
        {
            XmlDocument doc = new XmlDocument();
            RecruitApplicationShipFitting newFitting = new RecruitApplicationShipFitting();

            //Load the XMl Data based on the xmlData string representation
            doc.LoadXml(XML);

            try
            {
                //Set the new fittings properties
                newFitting.Active = true;
                newFitting.Description = doc.GetElementsByTagName("description")[0].Attributes["value"].Value.ToString();
                newFitting.ShipType = doc.GetElementsByTagName("shipType")[0].Attributes["value"].Value.ToString();
                newFitting.Name = doc.GetElementsByTagName("fitting")[0].Attributes["name"].Value.ToString().Replace(newFitting.ShipType + " - ", "");
                //A property containing the raw XML data as string format. Used later for retrieval etc.
                newFitting.XMLData = XML;
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst trying to upload a new ship fitting. Exn: {0}", exn.Message));
                return View("~/Views/Shared/CustomError.cshtml", null, "An error occured with processing your file as it may be malformed. Please try again.");
            }

            try
            {
                //If it got this far, the XML transformation and reading was successful, so add it to the database
                _recruitmentRepository.AddNewFitting(newFitting);
                _logger.Info(string.Format("Recruit application fitting for {0} called {1} was successfully added.", newFitting.ShipType, newFitting.Name));
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst attempting to add a new recruit application fitting. Exn: {0}", exn.Message));
                return View("~/Views/Shared/CustomError.cshtml", null, "An error occured whilst adding the new fitting. Please try again or contact an Admin/Director.");
            }
            return RedirectToAction("FittingManager");
        }

        /// <summary>
        /// Returns the XML data for the fitting in string format
        /// </summary>
        /// <param name="id">ID of fitting to display</param>
        /// <returns>XML String of fitting</returns>
        public string ViewXML(int id)
        {
            RecruitApplicationShipFitting fitting = _recruitmentRepository.GetShipFittingByID(id);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(fitting.XMLData);

            //Sets the content type to xml to allow for inbrowser rendering
            Response.ContentType = "text/xml";
            return doc.InnerXml;            
        }

        /// <summary>
        /// Downloads the XML fitting file
        /// </summary>
        /// <param name="id">ID of fitting to download</param>
        /// <returns>Download command for XML file of fitting</returns>
        public FileResult DownloadXML(int id)
        {
            RecruitApplicationShipFitting fitting = _recruitmentRepository.GetShipFittingByID(id);
            //Return a new file download containing the XML file of the fitting
            return File(Encoding.UTF8.GetBytes(fitting.XMLData), "application/xml", fitting.ShipType + " - " + fitting.Name);
        }

        /// <summary>
        /// Deletes a fitting based on its Id
        /// </summary>
        /// <param name="id">Id of fitting to remove</param>
        /// <returns>Redirect to the main FittingManager method</returns>
        public ActionResult DeleteFitting(int id)
        {
            try
            {                
                _recruitmentRepository.DeleteFitting(id);
                _logger.Info(string.Format("Fitting for {0} was successfully deleted.", id));
            }
            catch (Exception exn)
            {
                _logger.Warn(string.Format("An error occured whilst attempting to delete fitting - ID : {0}. Exn: {1}", id, exn.Message));
                return View("~/Shared/CustomError.cshtml", "An error occured whilst deleting the fitting. Please try again or contact an Admin/Director.");
            }
            return RedirectToAction("FittingManager");
        }

        /// <summary>
        /// Sets the status of a fitting based on its id
        /// </summary>
        /// <param name="id">Id of fitting to update</param>
        /// <param name="status">Status to update the fitting to.</param>
        public void SetFittingStatus(int id, bool status)
        {
            try
            {
                _recruitmentRepository.UpdateFittingStatus(id, status);
                _logger.Info(string.Format("Fitting {0} was successfully updated to status: {1}", id, status));
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst attempting to update the status of the fitting: {0}. Exn: {1}", id, exn.Message));                
            }
        }

        /// <summary>
        /// Main Maintenance Administration screen
        /// </summary>
        /// <returns>Administration view for Maintenance</returns>
        public ActionResult Maintenance()
        {
            MaintenanceViewModel viewModel = new MaintenanceViewModel()
            {
            };
            return View(viewModel);
        }

        /// <summary>
        /// Restarts the Instance of the web app, useful for flushing cache, forcing new code, removing session variables etc.
        /// </summary>
        /// <returns></returns>
        public string RestartInstance()
        {
            System.Web.HttpRuntime.UnloadAppDomain();

            _logger.Info("User forcefully restarted application instance.");
            return "Instance successfully restarted.";
        }
        
        /// <summary>
        /// Returns event log view containing all events
        /// </summary>
        /// <returns>Event log view</returns>
        public PartialViewResult GetEventLogs()
        {
            int totalPages = 0;
            IEnumerable<Log> eventLogs = _logRepository.GetPagedLogEntries(1, 10, out totalPages); //_logRepository.GetAllLogEntries();
            ViewBag.total = totalPages;
            return PartialView("Partials/_EventLog", eventLogs);
        }
    }
}
