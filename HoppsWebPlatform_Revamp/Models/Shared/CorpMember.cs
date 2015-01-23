using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HoppsWebPlatform_Revamp.Models
{
    public class CorpMember
    {
        [Key]
        public long PilotID { get; set; }
        public string PilotName { get; set; }
        public DateTime LastLogon { get; set; }
        public string Location { get; set; }
        public string Ship { get; set; }
        public long Roles { get; set; }
    }
}