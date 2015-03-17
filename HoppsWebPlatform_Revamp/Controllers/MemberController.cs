using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoppsWebPlatform_Revamp.Models;
using HoppsWebPlatform_Revamp.Utilities;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using HoppsWebPlatform_Revamp.DataAccess;

namespace HoppsWebPlatform_Revamp.Controllers
{
    public class MemberController : Controller
    {
        
    #region Properties
        private NLog.Logger _logger;
        private IApiRepository _apiRepository;
        private IAltRepository _altRepository;
        private ICorpMemberRepository _corpMemberRepository;
        #endregion

    #region Constructor
        /// <summary>
        /// Constructor for controller.
        /// </summary>
        public MemberController(IApiRepository apiRepository, IAltRepository altRepository, ICorpMemberRepository corpMemberRepository)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _apiRepository = apiRepository;
            _altRepository = altRepository;
            _corpMemberRepository = corpMemberRepository;
        }
        #endregion

        /// <summary>
        /// Main entry point for member, contains links to specific member pages.
        /// </summary>
        /// <returns>View of links</returns>
        [Authorize(Roles="Director")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Main view for viewing member profiles
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles="Director")]
        public ActionResult ViewMemberProfile()
        {
            ViewBag.PilotNames = _corpMemberRepository.GetAllCorpMembers().Select(x => x.PilotName);
            return View();
        }

        /// <summary>
        /// View for members profile
        /// </summary>
        /// <param name="pilotName">Pilot name to view profile of</param>
        /// <returns>Member profile for pilot</returns>
        [Authorize(Roles = "Director")]
        [HttpPost]
        public ActionResult ViewMemberProfile(string pilotName)
        {
            if (pilotName.Trim() != "")
            {
                if (APIHelper.EVE_DoesCharacterExist(pilotName))
                {
                    return PartialView("Partials/PilotProfile", GetPilotsProfile(pilotName));                        
                }
            }

            //See who says im not generating content.!!
            return Content("<b>Pilot does not exist.</b>");
        }

        /// <summary>
        /// Gets a pilots Profile
        /// </summary>
        /// <param name="pilotName">Pilots name</param>
        /// <returns>Pilots profile</returns>
        [Authorize]
        private PilotProfileViewModel GetPilotsProfile(string pilotName)
        {
            IEnumerable<Alt> chars = _altRepository.GetAllAltsForPilot(pilotName);
            string mainName = "";
            
            //Gets the main name for the pilot if its an alt
            mainName = (chars.Count() > 0)? chars.First().MainName : pilotName;

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

    }
}
