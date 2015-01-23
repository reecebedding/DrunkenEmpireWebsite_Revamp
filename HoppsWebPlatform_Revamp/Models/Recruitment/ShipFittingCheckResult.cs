using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Models
{
    public class ShipFittingCheckResult
    {
        public RecruitApplicationShipFitting ShipFitting { get; set; }

        public string PilotName { get; set; }
        public IEnumerable<SkillPrerequisite> MissingShipSkills { get; set; }
        public IEnumerable<SkillPrerequisite> MissingModuleSkills { get; set; }

        public string ErrorMessage { get; set; }
    }
}