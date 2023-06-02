using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace KYC_Portal_Admin.Utilities
{
    public static class Constants
    {
        public static string CONNECTION_STRING { get; private set; } = string.Empty;
        public static void SetAppSettings()
        {
            CONNECTION_STRING = ConfigurationManager.ConnectionStrings["KYC_DB"]?.ConnectionString;
        }
    }
}