using DAL;
using System.Web;
using System.Web.Mvc;

namespace Haberler.Auth
{
    public class AuthorizeEditor : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["Editor"] != null)
            {
                Editor edt = (Editor)httpContext.Session["Editor"];
                if (edt == null)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Home/Login");
        }
    }
}