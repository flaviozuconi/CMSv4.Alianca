using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.WebPages;

public static class HelperContext
{
    public static HtmlHelper Html
    {
        get { return ((WebViewPage)WebPageContext.Current.Page).Html; }
    }

    //public static IHtmlString RenderScripts(this HtmlHelper helper, params string[] additionalPaths)
    //{
    //    var page = helper.ViewDataContainer as WebPageExecutingBase;
    //    if (page != null && page.VirtualPath.StartsWith("~/"))
    //    {
    //        var virtualPath = "~/bundles" + page.VirtualPath.Substring(1);
    //        if (BundleTable.Bundles.GetBundleFor(virtualPath) == null)
    //        {
    //            var defaultPath = page.VirtualPath + ".js";
    //            BundleTable.Bundles.Add(new ScriptBundle(virtualPath).Include(defaultPath).Include(additionalPaths));
    //        }
    //        return MvcHtmlString.Create(@"<script src=""" + HttpUtility.HtmlAttributeEncode(BundleTable.Bundles.ResolveBundleUrl(virtualPath)) + @"""></script>");
    //    }
    //    return MvcHtmlString.Empty;
    //}
}

