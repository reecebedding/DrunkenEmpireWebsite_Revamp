using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HoppsWebPlatform_Revamp.Models
{
    public class ConfigurationViewModel
    {
        [Required(ErrorMessage="Director API Key is required")]
        public ApiKey DirectorAPIKey { get; set; }
        [Required(ErrorMessage="Corporation API Key Is Required")]
        public ApiKey CorporationAPIKey { get; set; }
    }
}