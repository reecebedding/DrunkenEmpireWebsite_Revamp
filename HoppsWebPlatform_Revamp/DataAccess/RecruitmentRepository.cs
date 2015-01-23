using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using Microsoft.Practices.EnterpriseLibrary.Data;
using HoppsWebPlatform_Revamp.Models;

namespace HoppsWebPlatform_Revamp.DataAccess
{
    public class RecruitmentRepository : DbContext, IRecruitmentRepository
    {
        #region Properties

        private NLog.Logger _logger;
        private IAppSettingsRepository _appSettingsRepository;

        #endregion

        private IRowMapper<RecruitmentApplicationQuestion> _questionRowMapperMain;
        private IMapBuilderContext<RecruitmentApplicationQuestion> CreateMainQuestionMapper()
        {
            return MapBuilder<RecruitmentApplicationQuestion>.MapNoProperties()
                .MapByName(x => x.ID)
                .MapByName(x => x.Description)                
                .MapByName(x => x.DataType)
                .MapByName(x => x.Active)                
                ;
        }

        private IRowMapper<RecruitmentApplicationQuestion> _questionWithAnswerRowMapper;
        private IMapBuilderContext<RecruitmentApplicationQuestion> CreateQuestionWithAnswerRowMapper()
        {
            return MapBuilder<RecruitmentApplicationQuestion>.MapNoProperties()
                .MapByName(x => x.ID)
                .MapByName(x => x.Description)
                .MapByName(x => x.Answer)
                ;
        }

        private IRowMapper<RecruitmentApplication> _applicationRowMapperMain;
        private IMapBuilderContext<RecruitmentApplication> CreateMainApplicationRowMapper()
        {
            return MapBuilder<RecruitmentApplication>.MapAllProperties();
        }

        private IRowMapper<RecruitApplicationShipFitting> _applicationShipFittingMapperMain;
        private IMapBuilderContext<RecruitApplicationShipFitting> CreateMainApplicationShipFittingMapper()
        {
            return MapBuilder<RecruitApplicationShipFitting>.MapAllProperties();
        }

        

        /// <summary>
        /// Constructor
        /// </summary>
        public RecruitmentRepository()
        {
            _questionRowMapperMain = CreateMainQuestionMapper().Build();
            _applicationRowMapperMain = CreateMainApplicationRowMapper().Build();
            _questionWithAnswerRowMapper = CreateQuestionWithAnswerRowMapper().Build();
            _applicationShipFittingMapperMain = CreateMainApplicationShipFittingMapper().Build();
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _appSettingsRepository = new AppSettingsRepository();
        }

        /// <summary>
        /// Gets all the active recruit application questions
        /// </summary>
        /// <returns>IEnumerable of active recruit application questions</returns>
        public IEnumerable<RecruitmentApplicationQuestion> GetAllActiveQuestions()
        {
            IEnumerable<RecruitmentApplicationQuestion> questions = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_RecruitApplication_Get_Active_Questions", _questionRowMapperMain);
            return questions;
        }

        /// <summary>
        /// Gets recruit question by question ID
        /// </summary>
        /// <param name="id">ID of question to retrieve</param>
        /// <returns>Recruit application question</returns>
        public RecruitmentApplicationQuestion GetQuestionByQuestionID(long id)
        {
            RecruitmentApplicationQuestion question = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_RecruitApplication_Get_Question_By_ID", _questionRowMapperMain, id).First();
            return question;
        }

        /// <summary>
        /// Gets recruit applications by pilotName
        /// </summary>
        /// <param name="pilotName">pilotName</param>
        /// <returns>IEnumerable of recruit applications for pilotName</returns>
        public IEnumerable<RecruitmentApplication> GetRecruitApplicationsByName(string pilotName)
        {
            IEnumerable<RecruitmentApplication> applications = null;
            applications = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_RecruitApplication_Get_Applications_Base_By_Name", _applicationRowMapperMain, pilotName);
            foreach (RecruitmentApplication item in applications)
            {
                item.Questions = GetApplicationQuestionAnswers(item.ID);
            }
            return applications;
        }

        /// <summary>
        /// Gets a recruit application by application ID
        /// </summary>
        /// <param name="appID">Application ID</param>
        /// <returns>Recruit application</returns>
        public RecruitmentApplication GetRecruitApplicationByID(long appID)
        {
            RecruitmentApplication application = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_RecruitApplication_Get_Application_By_ID", _applicationRowMapperMain, appID).First();
            application.Questions = GetApplicationQuestionAnswers(appID);
            return application;
        }

        /// <summary>
        /// Gets all active recruit applications
        /// </summary>
        /// <returns>IEnumerable for recruit applications that are currently active</returns>
        public IEnumerable<RecruitmentApplication> GetActiveApplications()
        {
            IEnumerable<RecruitmentApplication> applications = null;
            applications = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_RecruitApplication_Get_Active_Applications", _applicationRowMapperMain);
            return applications;
        }

        /// <summary>
        /// Returns all of the application questions stored.
        /// </summary>
        /// <returns>IEnumerable of questions</returns>
        public IEnumerable<RecruitmentApplicationQuestion> GetAllQuestions()
        {
            IEnumerable<RecruitmentApplicationQuestion> questions = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_RecruitApplication_Get_All_Questions", _questionRowMapperMain);
            return questions;
        }

        /// <summary>
        /// Updates an existing question 
        /// </summary>
        /// <param name="question">Question object containing details.</param>
        /// <returns>Switch based on whether the update was successful</returns>
        public bool EditQuestion(RecruitmentApplicationQuestion question)
        {
            var xx = HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_RecruitApplication_Edit_Question", question.ID, question.Description, question.DataType, question.Active);
            int result = (int)HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_RecruitApplication_Edit_Question", question.ID, question.Description, question.DataType, question.Active);
            return result == 1;
        }

        /// <summary>
        /// Adds a new recruit application question
        /// </summary>
        /// <param name="question">Recruit application question to add</param>
        /// <returns>Switch based on whether the application question was added successfully</returns>
        public bool AddQuestion(RecruitmentApplicationQuestion question)
        {
            int result = (int)HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_RecruitApplication_Add_Question", question.Description, question.DataType, question.Active);
            return result == 1;
        }

        /// <summary>
        /// Creates a new recruit application
        /// </summary>
        /// <param name="app">Recruit application to add</param>
        /// <param name="appID">Application ID to return (out)</param>
        /// <returns>Switch based on whether the insert was successful</returns>
        public bool CreateApplication(RecruitmentApplication app, out long appID)
        {
            var retAppID = HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_RecruitApplication_Add_Application", app.ApplicantName, app.P1Recruiter, app.Notes);
            appID = Convert.ToInt64(retAppID);
            return (appID > 0);
        }

        /// <summary>
        /// Adds a question answer for an application
        /// </summary>
        /// <param name="appID">Application ID</param>
        /// <param name="questionID">Question ID</param>
        /// <param name="answer">Question answer</param>
        public void AddQuestionForApplication(long appID, long questionID, string answer)
        {
            HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_RecruitApplication_Add_Application_Question", appID, questionID, answer);
        }

        /// <summary>
        /// Gets all the questions with answers for a recruit application
        /// </summary>
        /// <param name="appID">Application ID to get questions for.</param>
        /// <returns>IEnumerable of application question answers</returns>
        public IEnumerable<RecruitmentApplicationQuestion> GetApplicationQuestionAnswers(long appID)
        {
            IEnumerable<RecruitmentApplicationQuestion> questions = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_RecruitApplication_Get_Application_Questions_By_AppID", _questionWithAnswerRowMapper, appID);
            return questions;
        }

        /// <summary>
        /// Updates recruit application with the results of a background check
        /// </summary>
        /// <param name="appID">Application ID</param>
        /// <param name="notes">Background check notes</param>
        /// <param name="action">Result of background check</param>
        /// <param name="activeUser">Active user performing the background check</param>
        public void UpdateApplicationBackgroundCheck(long appID, string notes, string action, string activeUser)
        {
            HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_RecruitApplication_Update_Application_Background_Check", appID, notes, action, activeUser);
        }

        /// <summary>
        /// Updates recruit application with the completion results
        /// </summary>
        /// <param name="appID">Application ID</param>
        /// <param name="result">Result of the final decision</param>
        /// <param name="activeUser">Active user performing the completion task</param>
        public void CompleteApplication(long appID, string result, string activeUser)
        {
            HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_RecruitApplication_Complete_Application", appID, result, activeUser);
        }

        /// <summary>
        /// Gets a list of the keywords that should be checked against in the background eve mail check
        /// </summary>
        /// <returns>List of hazardous keywords</returns>
        public IEnumerable<string> GetBackgroundCheckMailKeywords()
        {
            AppSetting setting = _appSettingsRepository.GetSettingByName("BackgroundCheckEVEMailWords");
            return setting.Value.Split(',');
        }

        /// <summary>
        /// Adds a new fitting for the recruit application background checks
        /// </summary>
        /// <param name="fitting">Ship fitting</param>
        public void AddNewFitting(RecruitApplicationShipFitting fitting)
        {
            HoppsWebPlatformRevampDatabase.ExecuteNonQuery("Web_RecruitApplication_Add_Ship_Fitting", fitting.Name, fitting.Description, fitting.Active, fitting.ShipType, fitting.XMLData);                
        }

        /// <summary>
        /// Gets a list of recruit application ship fittings based on status
        /// </summary>
        /// <param name="activeOnly">Switch for what fittings. True = Active only, False = Everything</param>
        /// <returns>Collection of recruit application ship fittings</returns>
        public IEnumerable<RecruitApplicationShipFitting> GetShipFittingsByActivity(bool activeOnly)
        {
            IEnumerable<RecruitApplicationShipFitting> fittings = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_RecruitApplication_Get_Ship_Fittings", _applicationShipFittingMapperMain, activeOnly);
            return fittings;
        }

        /// <summary>
        /// Gets a specific recruit application ship fitting based on its id.
        /// </summary>
        /// <param name="FittingID">Id of fitting to retrieve</param>
        /// <returns>Recruit application ship fitting</returns>
        public RecruitApplicationShipFitting GetShipFittingByID(int fittingID)
        {
            return HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_RecruitApplication_Get_Ship_Fitting_By_ID", _applicationShipFittingMapperMain, fittingID).First();
        }

        /// <summary>
        /// Deletes a recruit application ship fitting based on its id
        /// </summary>
        /// <param name="fittingID">If of fitting to remove</param>
        public void DeleteFitting(int fittingID)
        {
            HoppsWebPlatformRevampDatabase.ExecuteNonQuery("Web_RecruitApplication_Remove_Ship_Fitting", fittingID);
        }

        /// <summary>
        /// Updates the status of a recruit application ship fitting based on its id
        /// </summary>
        /// <param name="fittingID">Id of fitting to update the status of</param>
        /// <param name="status">Status to update the fitting to</param>
        public void UpdateFittingStatus(int fittingID, bool status)
        {
            HoppsWebPlatformRevampDatabase.ExecuteNonQuery("Web_RecruitApplication_Update_Ship_Fitting_Status", fittingID, status);
        }

        
    }
}