using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HoppsWebPlatform_Revamp.Models
{
    public class Alt
    {
        [Key]
        public int Id { get; set; }
        public string MainName { get; set; }
        public string AltName { get; set; }
        public string AltPurpose { get; set; }
    }
}