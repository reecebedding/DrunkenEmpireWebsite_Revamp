using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Models
{
    public class WalletJournalEntry
    {
        public DateTime TimeStamp { get; set; }
        public string Sender { get; set; }
        public string Reciever { get; set; }
        public int Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
    }
}