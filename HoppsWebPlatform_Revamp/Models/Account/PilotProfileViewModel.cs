using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Models
{
    public class PilotProfileViewModel
    {
        public PilotProfileViewModel(string pilotName)
        {
            PilotName = pilotName;
        }

        public string PilotName { get; set; }
        public long PilotID { get; set; }
        public IEnumerable<ApiKey> ApiKeys { get; set; }

    }
}