using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;
using Microsoft.Practices.EnterpriseLibrary.Data;
using HoppsWebPlatform_Revamp.Models;

namespace HoppsWebPlatform_Revamp.DataAccess
{
    public class CorpMemberRepository : DbContext, ICorpMemberRepository
    {
        #region Properties

        private NLog.Logger _logger;

        #endregion

        #region Mappers

        private IRowMapper<CorpMember> _rowMapperMain;
        private IMapBuilderContext<CorpMember> CreateMainMapper()
        {
            return MapBuilder<CorpMember>.MapAllProperties();
        }

        #endregion

        public CorpMemberRepository()
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            _rowMapperMain = CreateMainMapper().Build();
        }

        /// <summary>
        /// Gets a list of all corporation members from the roster
        /// </summary>
        /// <returns>List of corp members</returns>
        public IEnumerable<CorpMember> GetAllCorpMembers()
        {
            IEnumerable<CorpMember> members = HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_Corp_Members_Get_All_Members", _rowMapperMain);
            return members;
        }

        /// <summary>
        /// Adds a corp member to the roster
        /// </summary>
        /// <param name="member">Member to add to the roster</param>
        /// <returns>Bool based on whether the member was added successfully</returns>
        public bool AddCorpMember(CorpMember member)
        {
            int result = (int)HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_Corp_Members_Add_Corp_Member", member.PilotID, member.PilotName, member.LastLogon, member.Location, member.Ship, member.Roles);
            return result == 1;
        }

        /// <summary>
        /// Truncates the corp roster and re-adds based on members param
        /// </summary>
        /// <param name="members">List of members to add as new member roster</param>
        public void UpdateCorpRoster(IEnumerable<CorpMember> members)
        {
            HoppsWebPlatformRevampDatabase.ExecuteScalar(System.Data.CommandType.Text, "DELETE FROM CorpMembers");
            foreach (CorpMember member in members)
            {
                AddCorpMember(member);
            }
        }

    }
}