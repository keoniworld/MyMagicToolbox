using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MyMagicToolbox
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            var database = new MagicDatabase.CardDatabase(
                HttpContext.Current.Server.MapPath("~/App_Data")
                // @"F:\dropbox\Dropbox\Magic\MyMagicToolbox\MyMagicToolbox\bin\App_Data"
                );
            database.GetAllSets();
        }
    }
}
