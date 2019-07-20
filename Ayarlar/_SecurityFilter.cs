using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OdevToplamaProjesi.Web.Ayarlar
{
    public class _SecurityFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            if (HttpContext.Current.Session["Kullanici"] == null && (controllerName != "Login"))
            {
                filterContext.Result = new RedirectResult("/Login/Index");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}