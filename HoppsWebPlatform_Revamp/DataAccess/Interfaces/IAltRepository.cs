using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HoppsWebPlatform_Revamp.Models;

namespace HoppsWebPlatform_Revamp.DataAccess.Interfaces
{
    public interface IAltRepository
    {
        IEnumerable<Alt> GetAllAltsForPilot(string pilotName);
        Alt GetAlt(string altName);
        bool AddAlt(Alt newAlt);
    }
}
