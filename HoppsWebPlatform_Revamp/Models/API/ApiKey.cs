using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HoppsWebPlatform_Revamp.Models
{
    [DataContract]
    public class ApiKey
    {
        public int Id { get; set; }
        public string PilotName { get; set; }

        [Required]
        [DataMember(IsRequired=true)]
        public long KeyID { get; set; }
        [Required]
        [DataMember(IsRequired = true)]
        public string VCode { get; set; }
        public bool Valid { get; set; }
        public string Errors { get; set; }
    }
}