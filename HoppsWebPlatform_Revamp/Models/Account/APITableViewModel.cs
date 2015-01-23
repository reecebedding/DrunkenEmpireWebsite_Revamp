using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Models
{
    public class APITableViewModel
    {
        public IEnumerable<ApiKey> existingAPIKeys { get; set; }
        public ApiKey newAPIKey { get; set; }
    }
}