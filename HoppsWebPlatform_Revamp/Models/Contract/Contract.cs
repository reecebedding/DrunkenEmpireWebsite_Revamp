using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Models
{
    public class Contract
    {
        public int UID { get; set; }
        public string Creator { get; set; }
        public DateTime Created { get; set; }
        public decimal Total { get; set; }
        public decimal TotalBeforeReduction { get; set; }

        public string Status { get; set; }
        public string RejectReason { get; set; }
        public string ProcessedBy { get; set; }

        private IEnumerable<ContractContent> _content;
        public IEnumerable<ContractContent> Content 
        {
            get 
            {
                _content = _content ?? new List<ContractContent>();
                return _content;
            }
            set { _content = value; }
        }
    }
}