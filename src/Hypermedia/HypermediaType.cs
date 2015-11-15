using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hypermedia {
    public abstract class HypermediaType<T> : IHypermediaType where T : HypermediaType<T> {
        protected HypermediaType() {
            Links = new List<UnresolvedLink>();
            Errors = new List<string>();
            Properties = new Dictionary<string, IHypermediaType>();
        }

        [JsonIgnore]
        private T Self => (T)this;
        public IList<UnresolvedLink> Links { get; }
        public IList<string> Errors { get; }
        public IDictionary<string, IHypermediaType> Properties { get; } 


        public T WithSimpleUrl(string rel, string title, Uri href, bool ignore = false) {
            return AddLink(new SimpleUrlLink(rel, title, href), ignore);
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

        public T ConfigureProperty(Expression<Func<T, decimal?>> memberExpression, Action<NumberValue> configureAction) {
            var value = new NumberValue(5);
            configureAction(value);

            Properties.Add(GetPropertyName(memberExpression), value);
            return Self;
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

        private T AddLink(UnresolvedLink link, bool ignore) {
            if (ignore) {
                return Self;
            }

            Links.Add(link);
            return Self;
        }

        private static string GetPropertyName<TProperty>(Expression<Func<T, TProperty>> exp) {
            var expression = (MemberExpression)exp.Body;
            return expression.Member.Name;
        }
    }
}