using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HoppsWebPlatform_Revamp.Utilities
{
    public class GlobalValues
    {
        private readonly String name;
        public static readonly GlobalValues AnErrorOccured = new GlobalValues("An error occured, please try again or contact a director.");
        public static readonly GlobalValues ValidationValidMessage = new GlobalValues("valid-message");

        public GlobalValues(string errorMessage)
        {
            name = errorMessage;
        }

        public override string ToString()
        {         
            return name;
        }

    }
}