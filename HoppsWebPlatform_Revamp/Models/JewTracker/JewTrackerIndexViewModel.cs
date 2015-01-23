using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Models
{
    public class JewTrackerIndexViewModel
    {
        public Dictionary<string, decimal> pilotValues;
        public decimal totalValue;

        public string topTaxedPilot = "";
        public string topTaxedSystem = "";
        public decimal topTaxedAmount = 0.0m;
    }
}