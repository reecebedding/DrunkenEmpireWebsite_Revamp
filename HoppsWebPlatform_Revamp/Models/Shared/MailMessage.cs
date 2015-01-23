using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Models
{
    public class MailMessage
    {
        public long MessageID { get; set; }
        public long SenderID { get; set; }
        public string SenderName { get; set; }
        public DateTime SentDate { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public List<long> ToPilotIdList { get; set; }
        public List<long> ToCorpAllianceIdList { get; set; }
        public List<long> ToMailingIdList { get; set; }

        private Dictionary<long, string> _toPilotNameList;
        public Dictionary<long, string> ToPilotNameList { get { return _toPilotNameList ?? (_toPilotNameList = new Dictionary<long, string>()); } set { _toPilotNameList = value; } }
        private Dictionary<long, string> _toCorporationNameList;
        public Dictionary<long, string> ToCorporationNameList { get { return _toCorporationNameList ?? (_toCorporationNameList = new Dictionary<long, string>()); } set { _toCorporationNameList = value; } }
        private Dictionary<long, string> _toMailingListNameList;
        public Dictionary<long, string> ToMailingListNameList { get { return _toMailingListNameList ?? (_toMailingListNameList = new Dictionary<long, string>()); } set { _toMailingListNameList = value; } }
        
        
    }
}