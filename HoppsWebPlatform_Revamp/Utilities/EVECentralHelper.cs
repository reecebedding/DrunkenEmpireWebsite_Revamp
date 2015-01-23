using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace HoppsWebPlatform_Revamp.Utilities
{
    public static class EVECentralHelper
    {
        public enum Regions { Forge = 10000002 };

        /// <summary>
        /// Gets the average percentile for an item in a certain region
        /// </summary>
        /// <param name="regionID">Region ID</param>
        /// <param name="itemID">Item ID</param>
        /// <returns>Value of item</returns>
        public static decimal GetItemPercentilePriceForRegion(long regionID, int itemID)
        {
            XElement rootDoc = XElement.Load(string.Format("http://api.eve-central.com/api/marketstat?typeid={0}&regionlimit={1}", itemID, regionID));
            decimal marketValue = Convert.ToDecimal(rootDoc.Element("marketstat").Element("type").Element("buy").Element("percentile").Value);

            return marketValue;            
        }

        /// <summary>
        /// Gets the average percentile for an item in a certain region
        /// </summary>
        /// <param name="regionID">Region ID</param>
        /// <param name="itemID">Item ID</param>
        /// <returns>Value of item</returns>
        public static decimal GetItemPercentilePriceForRegion(Regions regionID, int itemID)
        {
            return GetItemPercentilePriceForRegion((int)regionID, itemID);
        }
    }
}