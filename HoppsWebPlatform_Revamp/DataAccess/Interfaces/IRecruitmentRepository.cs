using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoppsWebPlatform_Revamp.Models;

namespace HoppsWebPlatform_Revamp.DataAccess.Interfaces
{
    public interface IRecruitmentRepository
    {
        IEnumerable<RecruitmentApplicationQuestion> GetAllActiveQuestions();
        RecruitmentApplicationQuestion GetQuestionByQuestionID(long id);
        IEnumerable<RecruitmentApplication> GetRecruitApplicationsByName(string pilotName);
        RecruitmentApplication GetRecruitApplicationByID(long appID);
        IEnumerable<RecruitmentApplication> GetActiveApplications();
        IEnumerable<RecruitmentApplicationQuestion> GetAllQuestions();
        IEnumerable<RecruitmentApplicationQuestion> GetApplicationQuestionAnswers(long appID);
        bool EditQuestion(RecruitmentApplicationQuestion question);
        bool AddQuestion(RecruitmentApplicationQuestion question);
        bool CreateApplication(RecruitmentApplication app, out long appID);
        void AddQuestionForApplication(long appID, long questionID, string answer);
        void UpdateApplicationBackgroundCheck(long appID, string notes, string action, string activeUser);
        void CompleteApplication(long appID, string result, string activeUser);
        IEnumerable<string> GetBackgroundCheckMailKeywords();
        void AddNewFitting(RecruitApplicationShipFitting fitting);
        IEnumerable<RecruitApplicationShipFitting> GetShipFittingsByActivity(bool activeOnly);
        RecruitApplicationShipFitting GetShipFittingByID(int fittingID);
        void DeleteFitting(int fittingID);
        void UpdateFittingStatus(int fittingID, bool status);
    }
}
