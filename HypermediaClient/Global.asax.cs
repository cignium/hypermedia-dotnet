using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HypermediaClient {
    public class MvcApplication : HttpApplication {
        protected void Application_Start() {
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            RouteTable.Routes.MapMvcAttributeRoutes();
            RouteTable.Routes.RouteExistingFiles = true;
            RouteTable.Routes.IgnoreRoute("");
        }
    }
}