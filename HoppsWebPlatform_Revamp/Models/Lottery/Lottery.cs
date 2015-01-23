using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Models
{
    public class Lottery
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PrizeDescription { get; set; }
        public int PrizeID { get; set; }
        public string CreatedBy { get; set; }
        public string TopPapEarner { get; set; }

        public IEnumerable<LotteryTicket> Tickets { get; set; }
    }
}