using System.Web.Mvc;

namespace CMSApp.Helpers
{
    public interface IHandleError
    {
        ActionResult GetResult();
    }
}
