using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Hypermedia {
    public class HypermediaActionResult : ActionResult {
        private readonly IHypermediaResource _resource;

        public HypermediaActionResult(IHypermediaResource resource) {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            _resource = resource;
        }

        public override void ExecuteResult(ControllerContext context) {
            var resolver = new ControllerActionUrlResolver(
                new UrlHelper(context.RequestContext),
                context.HttpContext.Request.Url.Scheme,
                context.Controller.ControllerContext.RouteData.Values["controller"].ToString());

            // Todo add a way to register ILinkFilter
            var filters = new List<ILinkFilter>();
            var untypedLinksResolver = new UnResolvedLinkResolver(resolver, filters);
            var serializer = new HypermediaTypeSerializer(untypedLinksResolver);
            var instance = serializer.Serialize(_resource);

            var response = context.HttpContext.Response;
            response.ContentType = _resource.MediaType;
            response.Write(instance.ToString());
        }
    }
}