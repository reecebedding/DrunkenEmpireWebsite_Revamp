using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Models
{
    public class ContractViewModel
    {
        public IEnumerable<Contract> PersonalContracts { get; set; }
        public IEnumerable<Contract> AdminContracts { get; set; }
    }
}