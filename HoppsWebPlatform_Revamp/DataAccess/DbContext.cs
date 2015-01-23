using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common;
using Microsoft.Practices.Unity;

namespace HoppsWebPlatform_Revamp.DataAccess
{
    public abstract class DbContext
    {
        private Database _hoppsWebPlatformRevampDatabase;
        public Database HoppsWebPlatformRevampDatabase
        {
            get 
            {
                if (_hoppsWebPlatformRevampDatabase == null)
                    _hoppsWebPlatformRevampDatabase = new DatabaseProviderFactory().CreateDefault();

                return _hoppsWebPlatformRevampDatabase;
            }
            set { _hoppsWebPlatformRevampDatabase = value; }
        }
    }
}