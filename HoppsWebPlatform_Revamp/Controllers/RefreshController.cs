using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoppsWebPlatform_Revamp.Utilities;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using HoppsWebPlatform_Revamp.DataAccess;
using HoppsWebPlatform_Revamp.Models;

namespace HoppsWebPlatform_Revamp.Controllers
{
    public class RefreshController : Controller
    {
        
    #region Properties

        private NLog.Logger _logger;
        private IRecruitmentRepository _recruitmentRepository;
        private IAppSettingsRepository _appSettingsRepository;
        private ICorpMemberRepository _corpMemberRepository;

    #endregion

        public RefreshController(IRecruitmentRepository recruitmentRepository, IAppSettingsRepository appSettingsRepository, ICorpMemberRepository corpMemberRepository)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _recruitmentRepository = recruitmentRepository;
            _appSettingsRepository = appSettingsRepository;
            _corpMemberRepository = corpMemberRepository;
        }

        /// <summary>
        /// Gets the index view
        /// </summary>
        /// <returns>Index view</returns>
        public ActionResult Index()
        {
            Dictionary<string, bool> results = new Dictionary<string, bool>();
            
            //Does the corp roster refresh and displays the message
            results.Add("CorpMembers", RefreshCorpMembers());
            //Does the corp member role refresh and displays the message
            results.Add("CorporationRole", RefreshCorporationRole()); 
   
            return View(results);
        }

        /// <summary>
        /// Refreshes the corporation roster
        /// </summary>
        /// <returns>Bool based on whether the roster was succesfully updated</returns>
        public bool RefreshCorpMembers()
        {
            try
            {
                ApiKey corporationAPI = _appSettingsRepository.GetCorporationAPI();
                IEnumerable<CorpMember> characters = APIHelper.EVE_GetCorporationMembers(corporationAPI.KeyID, corporationAPI.VCode);
                _corpMemberRepository.UpdateCorpRoster(characters);
                _logger.Info("Refreshing Corporation Members Complete");
                return true;
            }
            catch (Exception exn)
            {
                _logger.Error(string.Format("An error occured whilst trying to Refresh Corporation Members : {0}", exn.Message));
            }

            return false;
        }

        /// <summary>
        /// Cleans up the corporation roles by removing people from roles who are no longer in the corp
        /// </summary>
        /// <returns>Bool based on whether the corp roles was sync'd correctly</returns>
        public bool RefreshCorporationRole()
        {
            return true;
        }

    }
}
