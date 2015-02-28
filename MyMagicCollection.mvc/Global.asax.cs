using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MyMagicCollection.Shared.Models;

namespace MyMagicCollection.mvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var watch = Stopwatch.StartNew();
            var count = StaticMagicData.CardDefinitions.Count();
            watch.Stop();
            Debug.WriteLine("Loading cards took " + watch.Elapsed);
            int debug = 0;
        }
    }
}