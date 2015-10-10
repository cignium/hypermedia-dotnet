using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Hypermedia {
    public class ControllerActionLink<T> : UnResolvedLink {
        public ControllerActionLink(string rel, string title, Expression<T> expression, string controller = null, object routeValues = null) : base(rel, title) {
            Expression = expression;
            Controller = controller;
            RouteValues = routeValues;
        }

        public Expression<T> Expression { get; }
        public string Controller { get; }
        public object RouteValues { get; }

        public override Uri Resolve(IControllerActionUrlResolver resolver) {
            return string.IsNullOrEmpty(Controller) ? resolver.Generate(Expression, RouteValues) : resolver.Generate(Controller, Expression, RouteValues);
        }

        public override bool IsValid(IEnumerable<ILinkFilter> filters) {
            return filters.Select(x => x.Filter(Controller, Expression)).All(x => x);
        }
    }
}