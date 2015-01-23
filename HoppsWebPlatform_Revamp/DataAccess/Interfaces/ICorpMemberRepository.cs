using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoppsWebPlatform_Revamp.Models;

namespace HoppsWebPlatform_Revamp.DataAccess.Interfaces
{
    public interface ICorpMemberRepository
    {
        IEnumerable<CorpMember> GetAllCorpMembers();
        void UpdateCorpRoster(IEnumerable<CorpMember> members);
    }
}
