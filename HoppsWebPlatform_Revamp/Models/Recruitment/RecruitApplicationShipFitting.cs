using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Models
{
    public class RecruitApplicationShipFitting
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string ShipType { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string XMLData { get; set; }
    }
}