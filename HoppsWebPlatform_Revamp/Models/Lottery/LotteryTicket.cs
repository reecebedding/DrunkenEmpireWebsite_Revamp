using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Models
{
    public class LotteryTicket
    {
        public int ID { get; set; }
        public int LotteryID { get; set; }
        public string PilotName { get; set; }
        public int PilotID { get; set; }
        public int TicketNumber { get; set; }
        public int WinnerPosition { get; set; }
    }
}