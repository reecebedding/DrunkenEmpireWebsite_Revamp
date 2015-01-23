using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HoppsWebPlatform_Revamp.Models
{
    public class RecruitmentApplication
    {
        [Key]
        public long ID { get; set; }
        public string ApplicantName { get; set; }

        public DateTime P1Timestamp { get; set; }
        public string P1Recruiter { get; set; }
        public DateTime P2Timestamp { get; set; }
        public string P2Recruiter { get; set; }

        public DateTime CompletionTimeStamp { get; set; }
        public string CompletedBy { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public bool Active { get; set; }

        private IEnumerable<ApiKey> _apiKeys { get; set; }
        public IEnumerable<ApiKey> ApiKeys { get { if (_apiKeys == null) { _apiKeys = new List<ApiKey>(); } return _apiKeys; } set { _apiKeys = value; } }
        public IEnumerable<RecruitmentApplicationQuestion> Questions { get; set; }
    }
}