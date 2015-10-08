using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace Hypermedia {
    public class ControllerActionUrlResolver : IControllerActionUrlResolver {
        public ControllerActionUrlResolver(UrlHelper urlHelper, string scheme, string defaultControllerName) {
            if (urlHelper == null) {
                throw new ArgumentNullException(nameof(urlHelper));
            }

            DefaultControllerName = defaultControllerName;
            Scheme = scheme;
            Url = urlHelper;
        }

        private string DefaultControllerName { get; }
        private string Scheme { get; }
        private UrlHelper Url { get; }

        public Uri Generate<T>(string controller, Expression<T> controllerAction, object routeValues = null) {
            var additionalRouteValues = ObjectToDictionary(routeValues);
            var methodCallExpression = (MethodCallExpression)controllerAction.Body;
            var action = methodCallExpression.Method.Name;
            var values = methodCallExpression.Method
                .GetParameters()
                .ToDictionary(p => p.Name, p => ValuateExpression(methodCallExpression, p));

            additionalRouteValues?.ToList().ForEach(kvp => values.Add(kvp.Key, kvp.Value));

            var url = Url.Action(action, controller, new RouteValueDictionary(values), Scheme);

            if (string.IsNullOrEmpty(url)) {
                throw new ArgumentException($"Route could not be found. Controller: {controller}, Action: {action}");
            }

            return new Uri(url);
        }

        public Uri Generate<T>(Expression<T> controllerAction, object routeValues) {
            return Generate(DefaultControllerName, controllerAction, routeValues);
        }

        private static IDictionary<string, object> ObjectToDictionary(object o) {
            return o?.GetType()
                .GetProperties()
                .ToDictionary(
                    prop => prop.Name,
                    prop => prop.GetValue(o, null));
        }

        private static object ValuateExpression(MethodCallExpression methodCallExpression, ParameterInfo parameter) {
            var argument = methodCallExpression.Arguments[parameter.Position];
            var constantExpression = argument as ConstantExpression;

            if (constantExpression != null) {
                return constantExpression.Value;
            }

            var lambdaExpression = Expression.Lambda(argument, Enumerable.Empty<ParameterExpression>());
            return lambdaExpression.Compile().DynamicInvoke();
        }
    }
}
