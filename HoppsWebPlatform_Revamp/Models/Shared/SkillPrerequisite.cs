using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HoppsWebPlatform_Revamp.Models
{
    public class SkillPrerequisite
    {
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        public int RequiredLevel { get; set; }
        public int CurrentLevel { get; set; }
    }
}