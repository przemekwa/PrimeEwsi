using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace PrimeEwsi
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/view-create")
                .Include("~/Scripts/jquery-ui.js", "~/Scripts/view-create.js" ));
                


            bundles.Add(new ScriptBundle("~/bundles/view-history").Include("~/Scripts/view-history.js"));
            bundles.Add(new ScriptBundle("~/bundles/view-autorization").Include("~/Scripts/view-autorization.js"));

            bundles.Add(new ScriptBundle("~/bundles/dataTables").Include("~/Scripts/DataTables/jquery.dataTables.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include("~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css-dataTables").Include("~/Content/DataTables/css/jquery.dataTables.css"));
            bundles.Add(new StyleBundle("~/Content/css")
                .Include(
                "~/Content/bootstrap.css",
                "~/Content/Site.css", 
                "~/Content/font-awesome.css",
                "~/Scripts/jquery-ui.css"
                ));

#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif

        }
    }
}
