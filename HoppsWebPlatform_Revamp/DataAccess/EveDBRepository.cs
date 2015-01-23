using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HoppsWebPlatform_Revamp.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;
using HoppsWebPlatform_Revamp.DataAccess.Interfaces;

namespace HoppsWebPlatform_Revamp.DataAccess
{
    public class EveDBRepository : DbContext, IEveDBRepository
    {
        #region Mappers
        private IRowMapper<SkillPrerequisite> _skillPrerequisitesMapperMain;
        private IMapBuilderContext<SkillPrerequisite> CreateSkillPrerequisitesMapper()
        {
            return MapBuilder<SkillPrerequisite>.MapAllProperties().DoNotMap(x => x.CurrentLevel);
        }
        #endregion

        public EveDBRepository()
        {
            _skillPrerequisitesMapperMain = CreateSkillPrerequisitesMapper().Build();
        }

        /// <summary>
        /// Gets a list of all the primary skill requirements for an item based on name or ID
        /// </summary>
        /// <param name="itemID">Item ID to get requirements for</param>
        /// <param name="itemName">Name of item to get</param>
        /// <returns>List of skill prerequisites</returns>
        public IEnumerable<SkillPrerequisite> GetItemsPrimaryRequirements(int? itemID, string itemName)
        {
            return HoppsWebPlatformRevampDatabase.ExecuteSprocAccessor("Web_EVE_Get_Primary_Skill_Requirements_For_Item", _skillPrerequisitesMapperMain, itemID, itemName);
        }

        /// <summary>
        /// Gets an Item name from ID
        /// </summary>
        /// <param name="itemID">Item ID to get the name of</param>
        /// <returns>Name of item</returns>
        public string GetItemNameByID(long itemID)
        {
            return (string)HoppsWebPlatformRevampDatabase.ExecuteScalar("Web_EVE_Get_Item_Name_By_ID", itemID);
        }

        /// <summary>
        /// Gets an ID of an item by item name
        /// </summary>
        /// <param name="itemName">Item name to get ID of</param>
        /// <returns>ID of item</returns>
        public long GetItemIDByName(string itemName)
        {
            throw new NotImplementedException();
        }
    }
}