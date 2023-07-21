using KYC_Portal_Admin.Utilities;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace KYC_Portal_Admin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Constants.SetAppSettings();
        }
        public void Application_BeginRequest(object sender, EventArgs e)
        {
            //HttpContext context = ((HttpApplication)sender).Context;
            //Constants.CurrentUser = context.Request.Cookies.GetObject<UserInfo>("userInfo") ?? new UserInfo();
        }
    }
}
