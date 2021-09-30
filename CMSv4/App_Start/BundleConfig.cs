using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace CMSApp.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/BundleAdmin/Scripts").Include(
                    "~/content/js/flex.js",
                    "~/content/js/plugins/pace/pace.js",
                    "~/content/js/plugins/popupoverlay/defaults.js",
                    "~/content/js/plugins/popupoverlay/jquery.popupoverlay.js",
                    "~/content/js/plugins/messenger/messenger-theme-flat.js",
                    "~/content/js/plugins/popupoverlay/logout.js"
                )
            );

            bundles.Add(new StyleBundle("~/BundleAdmin/Css").Include(
                    "~/content/css/style.css",
                    "~/content/css/plugins/pace/pace.css",
                    "~/content/css/plugins.css",
                    "~/Content/css/plugins/messenger/messenger.css",
                    "~/Content/css/plugins/messenger/messenger-theme-flat.css"
                )
            );

            // Code removed for clarity.
            BundleTable.EnableOptimizations = true;
        }
    }
}