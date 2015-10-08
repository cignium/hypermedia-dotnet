using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hypermedia {
    public abstract class HypermediaResult<T> : ActionResult {
        protected HypermediaResult() {
            UnResolvedLinks = new List<UnResolvedLink>();
            Links = new List<ResolvedLink>();
        }

        [JsonIgnore]
        public abstract string Profile { get; }
        protected abstract T Self { get; }
        [JsonIgnore]
        public string MediaType => $"application/vnd.cignium.resource+json;profile={Profile}";
        [JsonIgnore]
        public IList<UnResolvedLink> UnResolvedLinks { get; }
        public IList<ResolvedLink> Links { get; }

        public override void ExecuteResult(ControllerContext context) {
            var response = context.HttpContext.Response;
            var settings = new JsonSerializerSettings {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var resolver = new ControllerActionUrlResolver(
                new UrlHelper(context.RequestContext),
                context.HttpContext.Request.Url.Scheme,
                context.Controller.ControllerContext.RouteData.Values["controller"].ToString());

            foreach (var unResolvedLink in UnResolvedLinks) {
                Links.Add(new ResolvedLink(unResolvedLink.Rel, unResolvedLink.Title, unResolvedLink.Resolve(resolver)));
            }

            response.ContentType = MediaType;
            response.Write(JsonConvert.SerializeObject(this, settings));
        }

        public T WithSimpleUrl(string rel, string title, Uri href, bool ignore = false) {
            if (ignore) {
                return Self;
            }
            UnResolvedLinks.Add(new SimpleUrlLink(rel, title, href));
            return Self;
        }

        public T WithAction(Expression<Action> controllerAction, string title, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Action, title, routeValues, ignore);
        }

        public T WithParent(Expression<Action> controllerAction, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Parent, null, routeValues, ignore);
        }

        public T WithSelf(Expression<Action> controllerAction, string title, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Self, title, routeValues, ignore);
        }

        public T WithUpdate(Expression<Action> controllerAction, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Update, null, routeValues, ignore);
        }

        public T WithAction(Expression<Func<Task>> controllerAction, string title, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Action, title, routeValues, ignore);
        }

        public T WithParent(Expression<Func<Task>> controllerAction, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Parent, null, routeValues, ignore);
        }

        public T WithSelf(Expression<Func<Task>> controllerAction, string title, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Self, title, routeValues, ignore);
        }

        public T WithUpdate(Expression<Func<Task>> controllerAction, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Update, null, routeValues, ignore);
        }

        public T WithAction<TController>(Expression<Action<TController>> controllerAction, string title, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Action, title, routeValues, ignore);
        }

        public T WithParent<TController>(Expression<Action<TController>> controllerAction, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Parent, null, routeValues, ignore);
        }

        public T WithSelf<TController>(Expression<Action<TController>> controllerAction, string title, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Self, title, routeValues, ignore);
        }

        public T WithUpdate<TController>(Expression<Action<TController>> controllerAction, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Update, null, routeValues, false);
        }

        public T WithAction<TController>(Expression<Func<TController, Task>> controllerAction, string title, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Action, title, routeValues, ignore);
        }

        public T WithParent<TController>(Expression<Func<TController, Task>> controllerAction, object routeValues = null) {
            return AddLink(controllerAction, LinkRel.Parent, null, routeValues, false);
        }

        public T WithSelf<TController>(Expression<Func<TController, Task>> controllerAction, string title, object routeValues = null) {
            return AddLink(controllerAction, LinkRel.Self, title, routeValues, false);
        }

        public T WithUpdate<TController>(Expression<Func<TController, Task>> controllerAction, object routeValues = null, bool ignore = false) {
            return AddLink(controllerAction, LinkRel.Update, null, routeValues, ignore);
        }

        private T AddLink<TController>(Expression<Func<TController, Task>> controllerAction, string rel, string title, object routeValues, bool ignore) {
            return AddLink(new ControllerActionLink<Func<TController, Task>>(rel, title, controllerAction, typeof(TController).Name, routeValues), ignore);
        }

        private T AddLink<TController>(Expression<Action<TController>> controllerAction, string rel, string title, object routeValues, bool ignore) {
            return AddLink(new ControllerActionLink<Action<TController>>(rel, title, controllerAction, typeof(TController).Name, routeValues), ignore);
        }

        private T AddLink(Expression<Func<Task>> controllerAction, string rel, string title, object routeValues, bool ignore) {
            return AddLink(new ControllerActionLink<Func<Task>>(rel, title, controllerAction, routeValues: routeValues), ignore);
        }

        private T AddLink(Expression<Action> controllerAction, string rel, string title, object routeValues, bool ignore) {
            return AddLink(new ControllerActionLink<Action>(rel, title, controllerAction, routeValues: routeValues), ignore);
        }

        private T AddLink(UnResolvedLink link, bool ignore) {
            if (ignore) {
                return Self;
            }

            UnResolvedLinks.Add(link);
            return Self;
        }
    }
}
