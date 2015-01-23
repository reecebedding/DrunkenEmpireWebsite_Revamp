using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace HoppsWebPlatform_Revamp.Models
{
    [Serializable]
    public class ContractContent
    {
        [IgnoreDataMember]
        public int UID { get; set; }
        [IgnoreDataMember]
        public int ContractID { get; set; }
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        [IgnoreDataMember]
        public decimal PricePerUnit { get; set; }
        [IgnoreDataMember]
        public int Quantity { get; set; }
    }
}