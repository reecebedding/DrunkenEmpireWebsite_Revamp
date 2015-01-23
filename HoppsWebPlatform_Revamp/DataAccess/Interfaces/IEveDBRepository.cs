using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoppsWebPlatform_Revamp.Models;

namespace HoppsWebPlatform_Revamp.DataAccess.Interfaces
{
    public interface IEveDBRepository
    {
        IEnumerable<SkillPrerequisite> GetItemsPrimaryRequirements(int? itemID, string itemName);
        string GetItemNameByID(long itemID);
        long GetItemIDByName(string itemName);
    }
}
