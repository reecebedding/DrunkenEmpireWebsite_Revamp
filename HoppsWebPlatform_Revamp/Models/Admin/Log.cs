using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HoppsWebPlatform_Revamp.Models
{
    public class Log
    {
        [Key]
        public int Id { get; set; }
        public string TimeStamp { get; set; }
        public string IPAddress { get; set; }
        public string Level { get; set; }
        public string User { get; set; }
        public string CallSiteClass { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
    }
}